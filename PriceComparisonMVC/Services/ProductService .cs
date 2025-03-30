using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Product;
using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Services
{
    public class ProductService : IProductService
    {
        private readonly IApiService _apiService;
        private const int DEFAULT_PAGE_SIZE = 5;

        public ProductService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ProductPageModel> GetProductPageModelAsync(int productId, int feedbackPage)
        {
            var productResponseModel = await GetProductAsync(productId);
            var baseId = productResponseModel?.BaseProductId ?? 0;
            var baseProductResponseModel = await GetBaseProductAsync(baseId);
            int productCategoryId = baseProductResponseModel?.CategoryId ?? 0;
            var characteristics = await GetProductCharacteristicsAsync(productId);
            var shortCharacteristics = await GetShortCharacteristicsAsync(productId);
            var productImages = await GetProductImagesAsync(baseId);
            var categoryDetails = await GetCategoryDetailsAsync(baseId);
            var category = categoryDetails != null ? await GetCategoryAsync(categoryDetails.Id) : null;
            var breadcrumb = category != null ? await BuildBreadcrumbAsync(category) : new List<CategoryResponseModel>();
            var feedbacks = await GetFeedbacksAsync(baseId, feedbackPage);
            var sellerProductDetails = await GetSellerProductDetailsAsync(baseId);
            var productColors = await GetProductColorsAsync(baseId, productResponseModel?.ProductGroup?.Id ?? 0);
            var relatedProducts = await GetRelatedProductsAsync(productCategoryId, baseId);
            var compatibleProductModel = CreateCompatibleProductModel(productResponseModel, baseProductResponseModel, productId);
            var productVariants = await GetProductsByBaseProductIdAsync(baseId);

            return new ProductPageModel
            {
                Characteristics = characteristics ?? new List<ProductCharacteristicResponseModel>(),
                ShortCharacteristics = shortCharacteristics ?? new List<ProductCharacteristicGroupResponseModel>(),
                ProductImages = productImages ?? new List<ProductImageModel>(),
                CategoryBreadcrumb = breadcrumb ?? new List<CategoryResponseModel>(),
                Feedbacks = feedbacks?.Data ?? new List<FeedbackResponseModel>(),
                FeedbacksPaging = feedbacks ?? CreateEmptyFeedbacksPaging(),
                RelatedProducts = relatedProducts,
                SellerProductDetails = sellerProductDetails ?? new List<SellerProductDetailResponseModel>(),
                ProductResponseModel = compatibleProductModel,
                CategoryId = productCategoryId,
                CurrentFeedbackPage = feedbackPage,
                ProductColors = productColors,
                ProductVariants = productVariants ?? new List<ProductsByBaseProductResponseModel>()
            };
        }

        //отримання характеристик продукту по id
        private async Task<ProductNewResponseModel?> GetProductAsync(int productId)
        {
            try
            {
                return await _apiService.GetAsync<ProductNewResponseModel>($"api/Products/{productId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання продукту: {ex.Message}");
                return null;
            }
        }

        //базові характеристики по baseId
        private async Task<BaseProductResponseModel?> GetBaseProductAsync(int baseId)
        {
            if (baseId <= 0) return null;

            try
            {
                return await _apiService.GetAsync<BaseProductResponseModel>($"api/BaseProducts/{baseId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання базового продукту: {ex.Message}");
                return null;
            }
        }

        //усі характеристики по productId
        private async Task<List<ProductCharacteristicResponseModel>?> GetProductCharacteristicsAsync(int productId)
        {
            try
            {
                return await _apiService.GetAsync<List<ProductCharacteristicResponseModel>>($"api/ProductCharacteristics/{productId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання характеристик: {ex.Message}");
                return new List<ProductCharacteristicResponseModel>();
            }
        }

        //згруповані короткі характеристики по productId
        private async Task<List<ProductCharacteristicGroupResponseModel>?> GetShortCharacteristicsAsync(int productId)
        {
            try
            {
                return await _apiService.GetAsync<List<ProductCharacteristicGroupResponseModel>>($"/api/ProductCharacteristics/short-grouped/{productId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання коротких характеристик: {ex.Message}");
                return new List<ProductCharacteristicGroupResponseModel>();
            }
        }


        //отримання картинок по baseId
        private async Task<List<ProductImageModel>?> GetProductImagesAsync(int baseId)
        {
            if (baseId <= 0) return new List<ProductImageModel>();

            try
            {
                return await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{baseId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання зображень: {ex.Message}");
                return new List<ProductImageModel>();
            }
        }


        private async Task<CategoryDetailsResponseModel?> GetCategoryDetailsAsync(int baseId)
        {
            if (baseId <= 0) return null;

            try
            {
                return await _apiService.GetAsync<CategoryDetailsResponseModel>($"/api/Categories/getbyproduct/{baseId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання деталей категорії: {ex.Message}");
                return null;
            }
        }


        private async Task<CategoryResponseModel?> GetCategoryAsync(int categoryId)
        {
            try
            {
                return await _apiService.GetAsync<CategoryResponseModel>($"api/Categories/{categoryId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання категорії: {ex.Message}");
                return null;
            }
        }

        //будуемо "хлібні крихти"
        public async Task<List<CategoryResponseModel>> BuildBreadcrumbAsync(CategoryResponseModel category)
        {
            var breadcrumb = new List<CategoryResponseModel>();
            while (category != null)
            {
                breadcrumb.Insert(0, category);

                if (category.ParentCategoryId.HasValue)
                {
                    category = await _apiService.GetAsync<CategoryResponseModel>($"api/Categories/{category.ParentCategoryId.Value}");
                }
                else
                {
                    category = null;
                }
            }
            return breadcrumb;
        }

        //відгуки по baseId
        private async Task<FeedbacksPagedResponseModel?> GetFeedbacksAsync(int baseId, int feedbackPage)
        {
            if (baseId <= 0) return CreateEmptyFeedbacksPaging();

            try
            {
                return await _apiService.GetAsync<FeedbacksPagedResponseModel>($"api/Feedback/paginated/{baseId}?page={feedbackPage}&pageSize={DEFAULT_PAGE_SIZE}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні відгуків: {ex.Message}");
                return CreateEmptyFeedbacksPaging();
            }
        }


        private FeedbacksPagedResponseModel CreateEmptyFeedbacksPaging()
        {
            return new FeedbacksPagedResponseModel
            {
                Data = new List<FeedbackResponseModel>(),
                Page = 1,
                PageSize = DEFAULT_PAGE_SIZE,
                TotalItems = 0
            };
        }

        //варіанти по базовому продукту
        private async Task<List<ProductsByBaseProductResponseModel>?> GetProductsByBaseProductIdAsync(int baseId)
        {
            if (baseId <= 0) return null;

            try
            {
                return await _apiService.GetAsync<List<ProductsByBaseProductResponseModel>>($"api/Products/bybaseproduct/{baseId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання варіантів продукту: {ex.Message}");
                return null;
            }
        }


        //отримання продавців по baseId
        private async Task<List<SellerProductDetailResponseModel>?> GetSellerProductDetailsAsync(int baseId)
        {
            if (baseId <= 0) return new List<SellerProductDetailResponseModel>();

            try
            {
                var sellers = await _apiService.GetAsync<List<SellerProductDetailResponseModel>>($"api/SellerProductDetails/{baseId}");
                if (sellers != null)
                {
                    foreach (var seller in sellers)
                    {
                        if (string.IsNullOrEmpty(seller.StoreName))
                        {
                            seller.StoreName = "Невідомий магазин";
                        }
                        if (string.IsNullOrEmpty(seller.ProductStoreUrl))
                        {
                            seller.ProductStoreUrl = "#";
                        }
                    }
                }
                return sellers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання SellerProductDetails: {ex.Message}");
                return new List<SellerProductDetailResponseModel>();
            }
        }

        //кольори по baseId і productGroupId
        private async Task<List<ProductColorResponseModel>> GetProductColorsAsync(int baseId, int productGroupId)
        {
            if (baseId <= 0 || productGroupId <= 0)
            {
                return GetDefaultProductColors();
            }

            try
            {
                return await _apiService.GetAsync<List<ProductColorResponseModel>>(
                    $"api/ProductColor/byproductgroup?BaseProductId={baseId}&ProductGroupId={productGroupId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання кольорів продукту: {ex.Message}");
                return GetDefaultProductColors();
            }
        }


        private List<ProductColorResponseModel> GetDefaultProductColors()
        {
            return new List<ProductColorResponseModel>
            {
                new ProductColorResponseModel { Id = 0, ProductId = 1, Name = "чорний", HexCode = "#000000", GradientCode = null },
                new ProductColorResponseModel { Id = 0, ProductId = 67, Name = "фіолетовий", HexCode = "#7f4699", GradientCode = null },
                new ProductColorResponseModel { Id = 0, ProductId = 68, Name = "золотистий", HexCode = "#c99839", GradientCode = null },
                new ProductColorResponseModel { Id = 0, ProductId = 69, Name = "сірий", HexCode = "#999999", GradientCode = null }
            };
        }


        //список популярних продуктів у категорії categoryId
        private async Task<List<RelatedProductModel>> GetRelatedProductsAsync(int categoryId, int baseId)
        {
            var relatedProducts = new List<RelatedProductModel>();
            if (categoryId <= 0) return relatedProducts;

            try
            {
                var productsWrapper = await _apiService.GetAsync<ProductsResponseWrapper>(
                    $"api/Products/bycategorypaginated/{categoryId}?page=1&pageSize=5");

                var products = new List<ProductToCategoriesListModel>();

                if (productsWrapper?.Data != null)
                {
                    foreach (var relatedProduct in productsWrapper.Data)
                    {
                        var convertedProduct = new ProductToCategoriesListModel
                        {
                            Id = relatedProduct.ProductGroups.FirstOrDefault()?.FirstProductId ?? relatedProduct.BaseProductId,
                            Title = relatedProduct.Title,
                            Description = relatedProduct.Description,
                            CategoryId = relatedProduct.CategoryId
                        };

                        try
                        {
                            var relatedProductImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{relatedProduct.BaseProductId}");
                            convertedProduct.ImageUrl = relatedProductImages?.FirstOrDefault()?.ImageUrl;
                        }
                        catch (Exception ex)
                        {
                            convertedProduct.ImageUrl = null;
                        }
                        products.Add(convertedProduct);
                    }
                }

                var relatedProductsFiltered = products.Where(p => p.Id != baseId).Take(4).ToList();

                foreach (var relatedProduct in relatedProductsFiltered)
                {
                    var imagesForRelated = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{relatedProduct.Id}");
                    var imageUrl = imagesForRelated?.FirstOrDefault()?.ImageUrl ?? "placeholder.jpg";

                    var priceInfo = await GetProductPriceAsync(relatedProduct.Id);

                    relatedProducts.Add(new RelatedProductModel
                    {
                        Id = relatedProduct.Id,
                        ProductName = relatedProduct.Title,
                        MinPrice = priceInfo.MinPriceValue,
                        MaxPrice = priceInfo.MaxPriceValue,
                        ImageUrl = imageUrl,
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання пов'язаних продуктів: {ex.Message}");
            }

            return relatedProducts;
        }

        //мінімальна і максимальна ціна продукту по productId
        private async Task<MinMaxPriceModel> GetProductPriceAsync(int productId)
        {
            try
            {
                return await _apiService.GetAsync<MinMaxPriceModel>($"api/SellerProductDetails/minmaxprices/{productId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання цін для товару {productId}: {ex.Message}");
                return new MinMaxPriceModel { ProductId = productId, MinPriceValue = 0, MaxPriceValue = 0 };
            }
        }

        //перетворення на сумісну модель
        private ProductResponseModel CreateCompatibleProductModel(ProductNewResponseModel? productResponseModel, BaseProductResponseModel? baseProductResponseModel, int productId)
        {
            if (productResponseModel != null && baseProductResponseModel != null)
            {
                return new ProductResponseModel
                {
                    Id = productResponseModel.Id,
                    Title = baseProductResponseModel.Title,
                    Description = baseProductResponseModel.Description,
                    CategoryId = baseProductResponseModel.CategoryId,
                    Brand = baseProductResponseModel.Brand,
                    ModelNumber = productResponseModel.ModelNumber,
                    Gtin = productResponseModel.Gtin,
                    Upc = productResponseModel.Upc
                };
            }
            else
            {
                return new ProductResponseModel
                {
                    Id = productId,
                    Title = "Інформація недоступна",
                    Description = "Опис товару недоступний",
                    Brand = "Невідомий бренд"
                };
            }
        }

        // Клас для отримання мінімальної та максимальної ціни товару
        public class MinMaxPriceModel
        {
            public int ProductId { get; set; }
            public decimal MinPriceValue { get; set; }
            public decimal MaxPriceValue { get; set; }
        }
    }
}