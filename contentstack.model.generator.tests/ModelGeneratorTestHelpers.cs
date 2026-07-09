using System;
using System.Collections.Generic;
using System.Text.Json;
using contentstack.model.generator;
using contentstack.model.generator.Model;
using Contentstack.Model.Generator.Model;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// Shared fixture builders for ModelGenerator code-generation tests.
    /// Not a test class - contains no [Fact]/[Theory] methods.
    /// </summary>
    internal static class ModelGeneratorTestHelpers
    {
        // Older than the "Apr, 04 2019" threshold GetDatatypeForField checks for "reference" fields.
        private static readonly DateTime PreReferenceVersionThreshold = DateTime.Parse("2018-01-01");

        internal static ModelGenerator CreateGenerator(
            bool isNullable = false,
            DateTime? stackVersion = null,
            IEnumerable<Contenttype>? seedContentTypes = null)
        {
            var generator = new ModelGenerator
            {
                IsNullable = isNullable,
                stack = new StackResponse
                {
                    Settings = new StackSettings { version = stackVersion ?? PreReferenceVersionThreshold }
                }
            };

            if (seedContentTypes != null)
            {
                generator._contentTypes.AddRange(seedContentTypes);
            }

            return generator;
        }

        internal static MetaData Meta(
            bool refMultiple = false,
            bool refMultipleContentType = false,
            bool isMarkdown = false,
            bool isJsonRTE = false)
        {
            return new MetaData
            {
                RefMultiple = refMultiple,
                RefMultipleContentType = refMultipleContentType,
                IsMarkdown = isMarkdown,
                IsJsonRTE = isJsonRTE
            };
        }

        internal static Field TextField(string uid, string? displayName = null, bool isMultiple = false, bool isMarkdown = false, object? referenceTo = null)
        {
            return new Field
            {
                Uid = uid,
                DisplayName = displayName ?? uid,
                DataType = "text",
                IsMultiple = isMultiple,
                ReferenceTo = referenceTo,
                FieldMetadata = Meta(isMarkdown: isMarkdown)
            };
        }

        internal static Field NumberField(string uid, bool isMultiple = false, bool refMultiple = false)
        {
            return new Field
            {
                Uid = uid,
                DisplayName = uid,
                DataType = "number",
                IsMultiple = isMultiple,
                FieldMetadata = Meta(refMultiple: refMultiple)
            };
        }

        internal static Field BooleanField(string uid)
        {
            return new Field { Uid = uid, DisplayName = uid, DataType = "boolean", FieldMetadata = Meta() };
        }

        internal static Field DateField(string uid)
        {
            return new Field { Uid = uid, DisplayName = uid, DataType = "isodate", FieldMetadata = Meta() };
        }

        internal static Field FileField(string uid, bool isMultiple = false)
        {
            return new Field { Uid = uid, DisplayName = uid, DataType = "file", IsMultiple = isMultiple, FieldMetadata = Meta() };
        }

        internal static Field JsonField(string uid, bool isJsonRTE)
        {
            return new Field { Uid = uid, DisplayName = uid, DataType = "json", FieldMetadata = Meta(isJsonRTE: isJsonRTE) };
        }

        internal static Field LinkTypeField(string uid)
        {
            return new Field { Uid = uid, DisplayName = uid, DataType = "link", FieldMetadata = Meta() };
        }

        internal static Field UnknownTypeField(string uid)
        {
            return new Field { Uid = uid, DisplayName = uid, DataType = "some_unmapped_type", FieldMetadata = Meta() };
        }

        internal static Field ReferenceField(string uid, object referenceTo, bool refMultiple = false, bool refMultipleContentType = false, bool isMultiple = false)
        {
            return new Field
            {
                Uid = uid,
                DisplayName = uid,
                DataType = "reference",
                IsMultiple = isMultiple,
                ReferenceTo = referenceTo,
                FieldMetadata = Meta(refMultiple: refMultiple, refMultipleContentType: refMultipleContentType)
            };
        }

        internal static Field GlobalFieldField(string uid, object referenceTo, bool refMultiple = false, bool refMultipleContentType = false, bool isMultiple = false)
        {
            var field = ReferenceField(uid, referenceTo, refMultiple, refMultipleContentType, isMultiple);
            field.DataType = "global_field";
            return field;
        }

        internal static Field GroupField(string uid, string displayName, List<Field>? schema)
        {
            return new Field { Uid = uid, DisplayName = displayName, DataType = "group", Schema = schema, FieldMetadata = Meta() };
        }

        internal static Field BlocksField(string uid, string displayName, List<Contenttype> blocks)
        {
            return new Field { Uid = uid, DisplayName = displayName, DataType = "blocks", Blocks = blocks, FieldMetadata = Meta() };
        }

        internal static Contenttype MakeContentType(string uid, string title, List<Field>? schema = null, string? referenceTo = null)
        {
            return new Contenttype
            {
                Uid = uid,
                Title = title,
                Schema = schema ?? new List<Field>(),
                ReferenceTo = referenceTo
            };
        }

        // Field.ReferenceTo is typed `object` but production code pattern-matches `is JsonElement` -
        // a plain string/string[] would silently fall through to "object", so build real JsonElements.
        internal static object SingleRef(string uid) => JsonSerializer.SerializeToElement(uid);

        internal static object MultiRef(params string[] uids) => JsonSerializer.SerializeToElement(uids);
    }
}
