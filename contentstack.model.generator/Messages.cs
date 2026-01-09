namespace contentstack.model.generator
{
    /// <summary>
    /// Centralized console messages for the Contentstack Model Generator
    /// </summary>
    public static class Messages
    {
        // Error Messages
        public const string UnexpectedError = "Unexpected error occurred. See logs for details.";
        public const string OperationFailed = "Operation failed. See error details in logs.";
        public const string ApiCommunicationError = "API communication error. Check your network or configuration and try again.";
        public const string AuthenticationFailed = "Authentication failed. Verify your API key and auth token and try again.";
        
        // Path Messages
        public static string NoPathSpecified(string path) => $"No path specified. Generating files in the current working directory: {path}.";
        public static string OutputPathSpecified(string path) => $"Output path specified. Generating files at: {path}.";
        public static string OutputPathNotFound(string path) => $"Output path not found. Creating: {path}.";
        public static string OpeningOutputDirectory(string directory) => $"Opening output directory: {directory}.";
        
        // Content Types Messages
        public const string FetchingStackDetails = "Fetching stack details for the provided API key.";
        public static string FetchingContentTypes(string stackName) => $"Fetching content types from {stackName} stack.";
        public static string FoundContentTypes(int count) => $"Found {count} content types.";
        public static string FetchedContentTypes(int count) => $"Fetched {count} content types.";
        public static string TotalContentTypesFetched(int count) => $"Total content types fetched: {count}.";
        
        // Global Fields Messages
        public static string FetchingGlobalFields(string stackName) => $"Fetching global fields from stack: {stackName}.";
        public static string FoundGlobalFields(int count) => $"Found {count} global fields.";
        public static string FetchedGlobalFields(int count) => $"Fetched {count} global fields.";
        public static string TotalGlobalFieldsFetched(int count) => $"Total global fields fetched: {count}.";
        
        // File Generation Messages
        public const string GeneratingFiles = "Generating files from content types.";
        public const string FilesCreatedSuccessfully = "Files created successfully.";
        public static string SkippingFile(string fileName) => $"Skipping {fileName} file.";
        public static string AddingFile(string fileName, string directory) => $"Adding {fileName} file to {directory}";
        
        // Field Messages
        public static string FieldDataType(string dataType) => $"Field data type: {dataType}.";
        
        // Modular Blocks and Groups Messages
        public static string ExtractingModularBlocksInContentType(string contentTypeName) => $"Extracting modular blocks in {contentTypeName} Content Type";
        public static string ExtractingGroupsInContentType(string contentTypeName) => $"Extracting groups in {contentTypeName} Content Type";
        public static string ExtractingModularBlocksInGroup(string groupName) => $"Extracting modular blocks in {groupName} group.";
        public static string ExtractingGroupsInGroup(string groupName) => $"Extracting groups in {groupName} group.";
    }
}

