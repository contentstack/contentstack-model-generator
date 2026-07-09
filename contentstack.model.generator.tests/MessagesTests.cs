using contentstack.model.generator;
using Xunit;

namespace contentstack.model.generator.tests
{
    public class MessagesTests
    {
        [Fact]
        public void NoPathSpecified_FormatsPath()
        {
            Assert.Equal(
                "No path specified. Generating files in the current working directory: /tmp.",
                Messages.NoPathSpecified("/tmp"));
        }

        [Fact]
        public void OutputPathSpecified_FormatsPath()
        {
            Assert.Equal(
                "Output path specified. Generating files at: /tmp.",
                Messages.OutputPathSpecified("/tmp"));
        }

        [Fact]
        public void OutputPathNotFound_FormatsPath()
        {
            Assert.Equal("Output path not found. Creating: /tmp.", Messages.OutputPathNotFound("/tmp"));
        }

        [Fact]
        public void OpeningOutputDirectory_FormatsDirectory()
        {
            Assert.Equal("Opening output directory: /tmp.", Messages.OpeningOutputDirectory("/tmp"));
        }

        [Fact]
        public void FetchingContentTypes_FormatsStackName()
        {
            Assert.Equal("Fetching content types from MyStack stack.", Messages.FetchingContentTypes("MyStack"));
        }

        [Fact]
        public void FoundContentTypes_FormatsCount()
        {
            Assert.Equal("Found 5 content types.", Messages.FoundContentTypes(5));
        }

        [Fact]
        public void FetchedContentTypes_FormatsCount()
        {
            Assert.Equal("Fetched 5 content types.", Messages.FetchedContentTypes(5));
        }

        [Fact]
        public void TotalContentTypesFetched_FormatsCount()
        {
            Assert.Equal("Total content types fetched: 5.", Messages.TotalContentTypesFetched(5));
        }

        [Fact]
        public void FetchingGlobalFields_FormatsStackName()
        {
            Assert.Equal("Fetching global fields from stack: MyStack.", Messages.FetchingGlobalFields("MyStack"));
        }

        [Fact]
        public void FoundGlobalFields_FormatsCount()
        {
            Assert.Equal("Found 3 global fields.", Messages.FoundGlobalFields(3));
        }

        [Fact]
        public void FetchedGlobalFields_FormatsCount()
        {
            Assert.Equal("Fetched 3 global fields.", Messages.FetchedGlobalFields(3));
        }

        [Fact]
        public void TotalGlobalFieldsFetched_FormatsCount()
        {
            Assert.Equal("Total global fields fetched: 3.", Messages.TotalGlobalFieldsFetched(3));
        }

        [Fact]
        public void SkippingFile_FormatsFileName()
        {
            Assert.Equal("Skipping Blog.cs file.", Messages.SkippingFile("Blog.cs"));
        }

        [Fact]
        public void AddingFile_FormatsFileNameAndDirectory()
        {
            Assert.Equal("Adding Blog.cs file to /tmp", Messages.AddingFile("Blog.cs", "/tmp"));
        }

        [Fact]
        public void FieldDataType_FormatsDataType()
        {
            Assert.Equal("Field data type: text.", Messages.FieldDataType("text"));
        }

        [Fact]
        public void ExtractingModularBlocksInContentType_FormatsContentTypeName()
        {
            Assert.Equal(
                "Extracting modular blocks in Blog Content Type",
                Messages.ExtractingModularBlocksInContentType("Blog"));
        }

        [Fact]
        public void ExtractingGroupsInContentType_FormatsContentTypeName()
        {
            Assert.Equal(
                "Extracting groups in Blog Content Type",
                Messages.ExtractingGroupsInContentType("Blog"));
        }

        [Fact]
        public void ExtractingModularBlocksInGroup_FormatsGroupName()
        {
            Assert.Equal("Extracting modular blocks in Meta group.", Messages.ExtractingModularBlocksInGroup("Meta"));
        }

        [Fact]
        public void ExtractingGroupsInGroup_FormatsGroupName()
        {
            Assert.Equal("Extracting groups in Meta group.", Messages.ExtractingGroupsInGroup("Meta"));
        }

        [Fact]
        public void ConstantMessages_HaveExpectedValues()
        {
            Assert.Equal("Unexpected error occurred. See logs for details.", Messages.UnexpectedError);
            Assert.Equal("Operation failed. See error details in logs.", Messages.OperationFailed);
            Assert.Equal(
                "API communication error. Check your network or configuration and try again.",
                Messages.ApiCommunicationError);
            Assert.Equal(
                "Authentication failed. Verify your API key and auth token and try again.",
                Messages.AuthenticationFailed);
            Assert.Equal("Fetching stack details for the provided API key.", Messages.FetchingStackDetails);
            Assert.Equal("Generating files from content types.", Messages.GeneratingFiles);
            Assert.Equal("Files created successfully.", Messages.FilesCreatedSuccessfully);
        }
    }
}
