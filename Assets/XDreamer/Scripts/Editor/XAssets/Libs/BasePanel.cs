namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class BasePanel
    {
        public virtual void Initialize(){ }

        public virtual void Update(){ }

        public virtual void Render(){ }

        public virtual void OnOptionModified(AssetLibWindowOption alwo) { }

        public virtual void OnSelectedCategoryChanged(Category category, Category lastCategory) { }

        public virtual void OnSearchItems(string searchText, ESearchType searchType) { }

        public virtual void OnEditModeChange(bool edit) { }

        public virtual void OnEditModel(Model model) { }

        public virtual void OnVisibleModelChange(Model model) { }

        public virtual void OnCancelEdit(Model model) { }

        public virtual void OnCancelEditCategory(Model model) { }

        public virtual void OnSubmitEdit(Model model) { }

        public virtual void OnSelectPage(int index) { }

        public virtual void OnChangePlaceType(EPlaceType placeType) { }

        public virtual void OnSelectCategory(bool isNext) { }

        public virtual void OnExpandCategory() { }

        public virtual void OnSaveAll() { }
    }
}
