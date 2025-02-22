namespace Exadel.BookManagement.API.Helpers
{
    public class DeleteBulkResponseModel
    {
        public string DeletedIDs { get; set; }
        public string NonExistIDs { get; set; }
    }
}
