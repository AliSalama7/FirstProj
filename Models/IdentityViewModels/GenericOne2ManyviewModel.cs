namespace MoviesApp.Models.IdentityViewModels
{
    public class GenericOne2ManyviewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<CheckBoxViewModel> Items { get; set; }
    }
}
