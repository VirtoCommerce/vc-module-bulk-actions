namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using Omu.ValueInjecter;

    using EditorialReview = VirtoCommerce.BulkActionsModule.Core.Models.EditorialReview;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class EditorialReviewConverter
    {
        public static VC.EditorialReview ToCoreModel(this EditorialReview review)
        {
            var result = new VC.EditorialReview();
            result.InjectFrom(review);
            return result;
        }

        public static EditorialReview ToWebModel(this VC.EditorialReview review)
        {
            var result = new EditorialReview
                             {
                                 Content = review.Content,
                                 Id = review.Id,
                                 IsInherited = review.IsInherited,
                                 LanguageCode = review.LanguageCode,
                                 ReviewType = review.ReviewType
                             };

            return result;
        }
    }
}