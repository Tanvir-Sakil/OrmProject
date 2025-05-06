using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.SqlClient;

namespace Assignment2
{

    public class MyORM<G, T> where T : class, IEntity<G>, new()
    {
        private readonly string _connectionString;

        public MyORM(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Insert(T item)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            DebugPrint(" Inserting Entity...");
            InsertRecursive<T, G>(item, connection, null, default);
        }
        public void Update(T item)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            DebugPrint("Updating Entity...");
            UpdateRecursive(item, connection);
        }
        public void Delete(T item)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            DebugPrint("2-Deleting Entity...");
            DeleteRecursive(item, connection);
        }
        public void Delete(G id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            DebugPrint($"3-Deleting Entity with ID: {id}");
            var item = GetById(id);
            if (item != null)
            {
               
                DeleteRecursive(item, connection);
            }
            else
            {
                Console.WriteLine("\bThere is no item with this Id");
            }
        }
        public T GetById(G id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            DebugPrint($"4-Getting Entity by ID: {id}");
            return GetByIdRecursive(typeof(T), id, connection) as T;
        }
        public List<T> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            DebugPrint("5-Getting All Entities...");
            return GetAllRecursive(typeof(T), connection) as List<T>;
        }
        private void InsertRecursive<T, G>(T item, SqlConnection connection, string parentTableName, G parentId)
        {
            
            var type = item.GetType();
            var tableName = type.Name;
            var properties = type.GetProperties();
            var columns = new List<string>();
            var values = new List<string>();
            var cont = 0;
            var propertyCont = properties.Length;

            if (parentTableName == null)
            {
                var parentColumn = $"{type.Name}Id";
             
                columns.Add(parentColumn);
          
                foreach (var property in properties)
                {
                    if (property.Name == "Id")
                    {
                        values.Add($"'{property.GetValue(item)}'");
                    }
                }
            }
            else
            {
                var parentColumn = $"{type.Name}Id";
               
                columns.Add(parentColumn);
                
                foreach (var property in properties)
                {
                    if (property.Name == "Id")
                    {
                        values.Add($"'{property.GetValue(item)}'");
                    }
                }


            }
            foreach (var property in properties)
            {

                if (parentTableName == null && property.PropertyType.Name == "Id")
                {
                    continue;
                }
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    continue;
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {

                    continue;
                }
                else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                {
                    continue;
                }
                else
                {
                    cont++;
                    if (property.Name == "Id") continue;
                    columns.Add(property.Name);
                  
                    values.Add($"'{property.GetValue(item)}'");
                }
            }



            if (parentTableName != null)
            {
                var parentColumn = $"{parentTableName}Id";
         
                columns.Add(parentColumn);
                values.Add($"'{parentId}'");
            }

            var insertQuery = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";
          
            using (var command = new SqlCommand(insertQuery, connection))
            {
                command.ExecuteNonQuery();
            }
            var columns1 = new List<string>();
            var values1 = new List<string>();
            if (propertyCont != cont)
            {
                bool isChildListItem = false;
                foreach (var property in properties)
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        cont++;
                        var list = (IEnumerable<object>)property.GetValue(item);
                        if (list != null)
                        {
                            foreach (var listItem in list)
                            {
                                var currentId = type.GetProperty("Id")?.GetValue(item);
                                InsertRecursive(listItem, connection, tableName, (G)currentId);
                            }
                        }
                        isChildListItem = true;
                    }
                    else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        cont++;
                        var nestedItem = property.GetValue(item);
                      
                        if (nestedItem != null)
                        {
                            var childIdProperty = nestedItem.GetType().GetProperty("Id");

                            if (childIdProperty != null)
                            {
                                var childIdValue = childIdProperty.GetValue(nestedItem);
                               
                                var childIdColumn = $"{property.Name}Id";
                                var Value = childIdValue;

                                if (Value != null)
                                {
                                    columns1.Add(childIdColumn);
                                    values1.Add($"'{childIdValue}'");
                                }
                                var currentId = type.GetProperty("Id")?.GetValue(item);
                                InsertRecursive(nestedItem,connection, tableName, (G)currentId);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                  
                }

                isChildListItem = false;
                if (isChildListItem && parentTableName != null)
                {
                    var parentColumn = $"{parentTableName}Id";
                  
                    columns1.Add(parentColumn);
                    values1.Add($"'{parentId}'");
                }
                if (columns1.Count > 0)
                {
                    var currentId = type.GetProperty("Id")?.GetValue(item);
                    var updateQuery = $"UPDATE {tableName} SET {string.Join(", ", columns1.Zip(values1, (c, v) => $"{c} = {v}"))} WHERE {tableName}Id = '{currentId}'";
                   
                    using (var command = new SqlCommand(updateQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                if (propertyCont == cont)
                {
                    return;
                }

            }
        }
        private void UpdateRecursive(object item, SqlConnection connection)
        {
            var type = item.GetType();
            var tableName = type.Name;
            var properties = type.GetProperties();

            var updates = new List<string>();
            G id = default;

            foreach (var property in properties)
            {
                if (property.Name == "Id")
                {
                    id = (G)property.GetValue(item);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = (IEnumerable<object>)property.GetValue(item);
                    if (list != null)
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var method = typeof(MyORM<G, T>).GetMethod(nameof(GetCollectionItems), BindingFlags.NonPublic | BindingFlags.Instance);
                        var genericMethod = method.MakeGenericMethod(elementType);
                        var existingItems = genericMethod.Invoke(this, new object[] { elementType, id, connection, type }) as IEnumerable<object>;

                        var existingItemsList = existingItems?.ToList() ?? new List<object>();
                        foreach (var listItem in list)
                        {
                            var listItemId = (G)listItem.GetType().GetProperty("Id")?.GetValue(listItem);
                            var existingItem = existingItemsList.FirstOrDefault(e => ((G)e.GetType().GetProperty("Id")?.GetValue(e)).Equals(listItemId));

                            if (existingItem == null)
                            {

                                InsertRecursive(listItem, connection, tableName, id);
                            }
                            else
                            {

                                if (HasChanges(listItem, existingItem))
                                {
                                    UpdateRecursive(listItem, connection);
                                }
                            }
                        }

                        foreach (var existingItem in existingItemsList)
                        {
                            var existingItemId = (G)existingItem.GetType().GetProperty("Id")?.GetValue(existingItem);
                            if (!list.Any(l => ((G)l.GetType().GetProperty("Id")?.GetValue(l)).Equals(existingItemId)))
                            {
                                DeleteRecursive(existingItem, connection);
                            }
                        }
                    }
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    var nestedItem = property.GetValue(item);
                    if (nestedItem != null)
                    {
                        UpdateRecursive(nestedItem, connection);
                    }
                }
                else
                {
                    var value = property.GetValue(item);
                    updates.Add($"{property.Name} = '{value}'");
                }
            }

            if (updates.Any())
            {
                var query = $"UPDATE {tableName} SET {string.Join(", ", updates)} WHERE {tableName}Id = '{id}'";
                using var command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }


        private bool HasChanges(object newItem, object existingItem)
        {

            if (newItem == null || existingItem == null)
            {
                return newItem != existingItem;
            }

            var type = newItem.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {

                if (property.GetMethod == null)
                {
                    continue;
                }


                if (property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                object newValue;
                object existingValue;

                try
                {

                    newValue = property.GetValue(newItem);
                    existingValue = property.GetValue(existingItem);
                }
                catch (Exception ex)
                {
                    continue;
                }

                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    if (HasChanges(newValue, existingValue))
                    {
                        return true;
                    }
                }

                else if (!Equals(newValue, existingValue))
                {
                    return true;
                }
            }

            return false;
        }
        public void DeleteRecursive(object item, SqlConnection connection)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var type = item.GetType();
            var tableName = type.Name;
            var properties = type.GetProperties();
            var idProperty = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            if (idProperty == null)
                throw new InvalidOperationException("The entity must have an 'Id' property.");

            var id = idProperty.GetValue(item);
            if (id == null)
                throw new InvalidOperationException("The 'Id' property cannot be null.");
            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var childList = (IEnumerable<object>)property.GetValue(item);
                    if (childList != null)
                    {
                        foreach (var childItem in childList)
                        {
                            DeleteRecursive(childItem, connection);
                        }
                    }
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    var nestedItem = property.GetValue(item);
                    if (nestedItem != null)
                    {
                        DeleteRecursive(nestedItem, connection);
                    }
                }
            }
            DeleteRecord(tableName, id, connection);
        }

        private void DeleteRecord(string tableName, object id, SqlConnection connection)
        {
            var foreignKeyConstraints = GetForeignKeyConstraints(tableName, connection);
            foreach (var constraint in foreignKeyConstraints)
            {
                var childTable = constraint.ChildTable;
                var childColumn = constraint.ChildColumn;
                var deleteChildQuery = $"DELETE FROM {childTable} WHERE {childColumn} = '{id}'";
                using (var command = new SqlCommand(deleteChildQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            var deleteQuery = $"DELETE FROM {tableName} WHERE {tableName}Id = '{id}'";
            using (var command = new SqlCommand(deleteQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private List<ForeignKeyConstraint> GetForeignKeyConstraints(string tableName, SqlConnection connection)
        {
            var constraints = new List<ForeignKeyConstraint>();
            var query = @"
            SELECT 
                fk.name AS ConstraintName,
                OBJECT_NAME(fk.parent_object_id) AS ChildTable,
                COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ChildColumn,
                OBJECT_NAME(fk.referenced_object_id) AS ParentTable,
                COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ParentColumn
            FROM 
                sys.foreign_keys fk
            INNER JOIN 
                sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            WHERE 
                OBJECT_NAME(fk.referenced_object_id) = @TableName"
            ;

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TableName", tableName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        constraints.Add(new ForeignKeyConstraint
                        {
                            ConstraintName = reader["ConstraintName"].ToString(),
                            ChildTable = reader["ChildTable"].ToString(),
                            ChildColumn = reader["ChildColumn"].ToString(),
                            ParentTable = reader["ParentTable"].ToString(),
                            ParentColumn = reader["ParentColumn"].ToString()
                        });
                    }
                }
            }

            return constraints;
        }


        private object GetByIdRecursive(Type type, G id, SqlConnection connection)
        {
            var tableName = type.Name;
            var properties1 = type.GetProperties();

            var query = $"SELECT * FROM {tableName} WHERE {tableName}Id = '{id}'";
            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var item = Activator.CreateInstance(type);
                foreach (var property in properties1)
                {
                    if (property.PropertyType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var method = typeof(MyORM<G, T>).GetMethod(nameof(GetCollectionItems), BindingFlags.NonPublic | BindingFlags.Instance);
                        if (method != null)
                        {

                            var genericMethod = method.MakeGenericMethod(elementType);
                            var collectionItems = genericMethod.Invoke(this, new object[] { elementType, id, connection, type });
                            property.SetValue(item, collectionItems);
                        }
                    }
                    else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                            // Console.WriteLine(property.Name);
                            // var nestedId = reader\[\$"{property.Name}Id"];
                             G? nestedId = reader[$"{property.Name}Id"] != DBNull.Value
                                           ? (G)reader[$"{property.Name}Id"]
                                         : default;

                                  var nestedItem = GetByIdRecursive(property.PropertyType, (G)nestedId, connection);
                                  property.SetValue(item, nestedItem);

                            }

                    else if (property.Name =="Id")
                    {
                        var columnName = $"{tableName}{property.Name}";
                        property.SetValue(item, Guid.Parse(reader[columnName].ToString()));
                    }
                    else
                    {
                        if (property.Name.Substring(property.Name.Length - 2)=="Id")
                        {
                            if ((reader[property.Name] == DBNull.Value))
                            {
                                G propertyId = (G)(object)Guid.Empty;
                                property.SetValue(item, propertyId);
                            }
                            else
                            {
                                property.SetValue(item, reader[property.Name]);
                            }
                        }
                        else
                        {
                            property.SetValue(item, reader[property.Name]);
                        }
                    }

                }

                return item;
            }

            return null;
        }

        private List<TElement> GetCollectionItems<TElement>(Type elementType, G parentId, SqlConnection connection, Type parentType = null) where TElement : class, IEntity<G>, new()
        {
            var tableName = elementType.Name;
            var name = typeof(T).Name;
            var query = "";
            if (parentType != null)
            {
                var childIdProperty = parentType.GetType().GetProperty("Id");

                if (childIdProperty != null)
                {
                    var childIdValue = childIdProperty.GetValue(parentType);
                }
                query = $"SELECT * FROM {tableName} WHERE {parentType.Name}Id = '{parentId}'";
            }
            else
            {
                query = $"SELECT * FROM {tableName} WHERE {typeof(T).Name}Id = '{parentId}'";
            }

            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var items = new List<TElement>();

            while (reader.Read())
            {
                var item = new TElement();

                foreach (var property in elementType.GetProperties())
                {
                    if (property.PropertyType.IsGenericType &&
                      typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
                        property.PropertyType != typeof(string))
                    {
                        var childElementType = property.PropertyType.GetGenericArguments()[0];
                        var method = typeof(MyORM<G, T>).GetMethod(nameof(GetCollectionItems), BindingFlags.NonPublic | BindingFlags.Instance);

                        if (method != null)
                        {
                            var genericMethod = method.MakeGenericMethod(childElementType);
                            var currentEntityId = (G)elementType.GetProperty("Id").GetValue(item);

                            var collectionItems = genericMethod.Invoke(this, new object[] { childElementType, currentEntityId, connection, elementType });
                            property.SetValue(item, collectionItems);
                        }
                    }
                    else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        var nestedId = reader[$"{property.Name}Id"];
                        var nestedItem = GetByIdRecursive(property.PropertyType, (G)nestedId, connection);
                        property.SetValue(item, nestedItem);
                    }
                    else if (property.Name == "Id")
                    {
                        var columnName = $"{tableName}{property.Name}";
                        property.SetValue(item, Guid.Parse(reader[columnName].ToString()));
                    }
                    else
                    {
                        property.SetValue(item, reader[property.Name]);
                    }
                }

                items.Add(item);
            }

            return items;
        }
        private object GetAllRecursive(Type type, SqlConnection connection)
        {
            var tableName = type.Name;
            var properties = type.GetProperties();

            var query = $"SELECT * FROM {tableName}";

            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var listType = typeof(List<>).MakeGenericType(type);
            var items = Activator.CreateInstance(listType) as IList;

            while (reader.Read())
            {
                var item = Activator.CreateInstance(type);

                foreach (var property in properties)
                {
                    if (property.PropertyType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType))
                    {

                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var method = typeof(MyORM<G, T>).GetMethod(nameof(GetCollectionItems), BindingFlags.NonPublic | BindingFlags.Instance );
                        if (method != null)
                        {

                            var genericMethod = method.MakeGenericMethod(elementType);
                            var collectionItems = genericMethod.Invoke(this, new object[] { elementType, reader[$"{type.Name}Id"], connection, type });
                            property.SetValue(item, collectionItems);
                        }
                    }
                    else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {

                        var nestedId = reader[$"{property.Name}Id"];
                        if (nestedId != DBNull.Value)
                        {
                            var nestedItem = GetByIdRecursive(property.PropertyType, (G)nestedId, connection);
                            property.SetValue(item, nestedItem);
                        }
                    }
                    else
                    {
                        if (property.Name == "Id")
                        {
                            var columnName = $"{tableName}{property.Name}";
                            property.SetValue(item, Guid.Parse(reader[columnName].ToString()));
                        }
                        else if (reader[property.Name] != DBNull.Value)
                        {
                            property.SetValue(item, reader[property.Name]);
                        }
                    }
                }

                items.Add(item);
            }

            return items;
        }
        private void DebugPrint(string message)
        {
            //Console.WriteLine($"[DEBUG]: {message}");
        }
    }
}
