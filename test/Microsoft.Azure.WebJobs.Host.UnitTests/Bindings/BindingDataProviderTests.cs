﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.WebJobs.Host.UnitTests.Bindings
{
    public class BindingDataProviderTests
    {
        [Fact]
        public void FromType_IfObjectParamType_ReturnsNull()
        {
            // Arrange && Act
            IBindingDataProvider provider = BindingDataProvider.FromType(typeof(object));

            // Assert
            Assert.Null(provider);
        }

        [Fact]
        public void FromType_IfIntParamType_ReturnsNull()
        {
            // Simple type, not structured, doesn't produce any route parameters.
            // Arrange && Act
            IBindingDataProvider provider = BindingDataProvider.FromType(typeof(int));

            // Assert
            Assert.Null(provider);
        }

        [Fact]
        public void FromType_IfStructuredDataType_ReturnsValidContract()
        {
            // Arrange && Act
            IBindingDataProvider provider = BindingDataProvider.FromType(typeof(NestedDataType));

            // Assert
            Assert.NotNull(provider);
            Assert.NotNull(provider.Contract);
            
            var names = provider.Contract.Keys.ToArray();
            Array.Sort(names);
            var expected = new string[] { "IntProp", "Nested", "StringProp" };
            Assert.Equal(expected, names);
        }

        [Fact]
        public void GetBindingData_IfSimpleDataType_ReturnsValidBindingData()
        {
            // Arrange
            IBindingDataProvider provider = BindingDataProvider.FromType(typeof(SimpleDataType));

            // When JSON is a structured object, we can extract the fields as route parameters.
            string json = @"{ ""Name"" : 12, ""other"" : 13 }";
            object value = JsonConvert.DeserializeObject(json, typeof(SimpleDataType));

            // Act
            var bindingData = provider.GetBindingData(value);

            // Assert
            Assert.NotNull(bindingData);
            Assert.Equal(1, bindingData.Count);
            Assert.Equal(12, bindingData["Name"]);
        }

        [Fact]
        public void GetBindingData_IfComplexDataType_ReturnsBindingDataWithAllTypes()
        {
            // Arrange
            IBindingDataProvider provider = BindingDataProvider.FromType(typeof(ComplexDataType));

            // When JSON is a structured object, we can extract the fields as route parameters.
            string json = @"{
""a"":1,
""b"":[1,2,3],
""c"":{}
}";
            object value = JsonConvert.DeserializeObject(json, typeof(ComplexDataType));

            // Act
            var bindingData = provider.GetBindingData(value);

            // Assert
            Assert.NotNull(bindingData);

            // Only take simple types
            Assert.Equal(3, bindingData.Count);
            Assert.Equal(1, bindingData["a"]);
            Assert.Equal(new int[] { 1, 2, 3 }, bindingData["b"]);
            Assert.Equal(new Dictionary<string, object>(), bindingData["c"]);
        }

        [Fact]
        public void GetBindingData_IfDateProperty_ReturnsValidBindingData()
        {
            // Arrange
            IBindingDataProvider provider = BindingDataProvider.FromType(typeof(DataTypeWithDateProperty));

            // Dates with JSON can be tricky. Test Date serialization.
            DateTime date = new DateTime(1950, 6, 1, 2, 3, 30);

            var json = JsonConvert.SerializeObject(new { date = date });
            object value = JsonConvert.DeserializeObject(json, typeof(DataTypeWithDateProperty));

            // Act
            var bindingData = provider.GetBindingData(value);

            // Assert
            Assert.NotNull(bindingData);
            Assert.Equal(1, bindingData.Count);
            Assert.Equal(date, bindingData["date"]);
        }

        private class SimpleDataType
        {
            public int Name { get; set; }
        }

        private class ComplexDataType
        {
            public int a { get; set; }

            public int[] b { get; set; }

            public IDictionary<string, object> c { get; set; }
        }

        private class DataTypeWithDateProperty
        {
            public DateTime date { get; set; }
        }

        class NestedDataType
        {
            public int _field = 0; // skip: not a property

            public int IntProp { get; set; } // Yes

            public string StringProp { get; set; } // Yes

            public NestedDataType Nested { get; set; } // Yes

            public static int StaticIntProp { get; set; } // skip: not instance property

            private int PrivateIntProp { get; set; } // skip: private property
        }
    }
}
