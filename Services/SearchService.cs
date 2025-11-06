namespace Afterpelago.Services
{
    public class SearchService
    {
        public event Action OnDataChanged;
        private Check _searchItem;
        public Check SearchItem
        {
            get => _searchItem;
            set
            {
                _searchItem = value;
                OnDataChanged?.Invoke();
            }
        }
    }
}
