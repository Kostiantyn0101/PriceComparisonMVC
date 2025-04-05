using System.Collections.Generic;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Data
{
    public static class IndexContentData
    {
        // Окремий метод для отримання категорій
        public static List<CategoryModel> GetCategories()
        {
            return new List<CategoryModel>
            {
                new CategoryModel { Id = 24, CategoryName = "Аудіо", CategoryIconUrl = "images/category-audio.png" },
                new CategoryModel { Id = 22, CategoryName = "Гаджети", CategoryIconUrl = "~/images/category-game-controller.png" },
                new CategoryModel { Id = 23, CategoryName = "Комп'ютери", CategoryIconUrl = "~/images/category-computer.png" },
                new CategoryModel { Id = 25, CategoryName = "Фото", CategoryIconUrl = "~/images/category-photo.png" },
                new CategoryModel { Id = 26, CategoryName = "ТV", CategoryIconUrl = "~/images/category-tv.png" },
                new CategoryModel { Id = 27, CategoryName = "Побутова техніка", CategoryIconUrl = "~/images/category-refrigerator.png" },
                new CategoryModel { Id = 28, CategoryName = "Клімат", CategoryIconUrl = "~/images/category-climat.png" },
                new CategoryModel { Id = 29, CategoryName = "Дім", CategoryIconUrl = "~/images/category-home.png" },
                new CategoryModel { Id = 30, CategoryName = "Дитячі товари", CategoryIconUrl = "~/images/category-home.png" },
                new CategoryModel { Id = 31, CategoryName = "Авто", CategoryIconUrl = "~/images/category-car.png" },
                new CategoryModel { Id = 32, CategoryName = "Інструменти", CategoryIconUrl = "~/images/category-wrench.png" },
                new CategoryModel { Id = 33, CategoryName = "Туризм", CategoryIconUrl = "~/images/category-backpack.png" },
                new CategoryModel { Id = 34, CategoryName = "Спорт", CategoryIconUrl = "~/images/category-bicycle.png" },
                new CategoryModel { Id = 35,  CategoryName = "Моба та аксесуари", CategoryIconUrl = "~/images/category-dress.png" }
            };
        }

        // Методи для отримання товарів за категоріями
        public static List<ItemWhithUrlAndPriceModel> GetSmartphones()
        {
            return new List<ItemWhithUrlAndPriceModel>
            {
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/16Pro.jpg",
                    ProductDescription = "Apple iPhone 16 Pro Max ",
                    ProductPrice = "₴59999",
                    ProductId = "2",
                    Category = "Смартфони"
                },
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/S24.jpg",
                    ProductDescription = "Samsung Galaxy S24 Ultra 5G 12/512GB Black",
                    ProductPrice = "₴54999",
                    ProductId = "1",
                    Category = "Смартфони"
                },
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/Redmi13.jpg",
                    ProductDescription = "Xiaomi Note 13 Pro 5G",
                    ProductPrice = "₴28999",
                    ProductId = "4",
                    Category = "Смартфони"
                },
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/X6.jpg",
                    ProductDescription = "Poco X6 Pro",
                    ProductPrice = "₴19999",
                    ProductId = "3",
                    Category = "Смартфони"
                }
            };
        }

        public static List<ItemWhithUrlAndPriceModel> GetLaptops()
        {
            return new List<ItemWhithUrlAndPriceModel>
            {
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/Mac14.jpg",
                    ProductDescription = "MacBook Pro 14 (Space Black)",
                    ProductPrice = "₴129999",
                    ProductId = "38",
                    Category = "Ноутбуки"
                },
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/Lenovo.jpg",
                    ProductDescription = "Lenovo LOQ 15IAX9",
                    ProductPrice = "₴34999",
                    ProductId = "12",
                    Category = "Ноутбуки"
                },
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/Msi.jpg",
                    ProductDescription = "MSI Thin 15 B12VE",
                    ProductPrice = "₴49999",
                    ProductId = "34",
                    Category = "Ноутбуки"
                },
                new ItemWhithUrlAndPriceModel {
                    IconUrl = "images/popular-item/Acer.jpg",
                    ProductDescription = "Acer Nitro V 15 ANV15-41",
                    ProductPrice = "₴34999",
                    ProductId = "36",
                    Category = "Ноутбуки"
                }
            };
        }

        // Метод для отримання популярних категорій
        public static List<PopularCategoryModel> GetPopularCategories()
        {
            return new List<PopularCategoryModel>
            {
                new PopularCategoryModel { Name = "Смартфони", CategoryId = "1" },
                new PopularCategoryModel { Name = "Ноутбуки", CategoryId = "2" },
                new PopularCategoryModel { Name = "Планшети", CategoryId = "3" },
                new PopularCategoryModel { Name = "Смарт-Годинники", CategoryId = "4" },
                new PopularCategoryModel { Name = "Телевізори", CategoryId = "5" }
            };
        }

        // Метод для отримання словника продуктів за категоріями
        public static Dictionary<string, List<ItemWhithUrlAndPriceModel>> GetProductsByCategory()
        {
            return new Dictionary<string, List<ItemWhithUrlAndPriceModel>>
            {
                { "Смартфони", GetSmartphones() },
                { "Ноутбуки", GetLaptops() }
            };
        }

        // Основний метод для отримання вмісту індексної сторінки
        public static IndexContentModel GetIndexContent()
        {
            return new IndexContentModel
            {
                Categories = GetCategories(),

                PopulaCategoriesImages = new List<ItemToViewModel>
                {
                    new ItemToViewModel { Name = "left-up", IconUrl = "~/images/reclam-block/left-up.png", CategoryId = "29" },
                    new ItemToViewModel { Name = "left-middle", IconUrl = "~/images/reclam-block/left-midle.png", CategoryId = "82" },
                    new ItemToViewModel { Name = "left-down", IconUrl = "~/images/reclam-block/left-down.png", CategoryId = "105" },
                    new ItemToViewModel { Name = "middle-гu", IconUrl = "~/images/reclam-block/middle-up.png", CategoryId = "36" },
                    new ItemToViewModel { Name = "middle-middle-left", IconUrl = "~/images/reclam-block/middle-middle-left.png", CategoryId = "39" },
                    new ItemToViewModel { Name = "middle-middle-right", IconUrl = "~/images/reclam-block/middle-middle-right.png", CategoryId = "146" },
                    new ItemToViewModel { Name = "middle-down", IconUrl = "~/images/reclam-block/middle-down.png", CategoryId = "101" },
                    new ItemToViewModel { Name = "right-up", IconUrl = "~/images/reclam-block/right-up.png", CategoryId = "38" },
                    new ItemToViewModel { Name = "right-middle", IconUrl = "~/images/reclam-block/right-middle.png", CategoryId = "159" },
                    new ItemToViewModel { Name = "right-down", IconUrl = "~/images/reclam-block/right-down.png", CategoryId = "157 " }
                },

                // За замовчуванням відображаємо смартфони
                PopularProducts = GetSmartphones(),

                // Додаємо словник для швидкого доступу до товарів за категоріями 
                ProductsByCategory = GetProductsByCategory(),

                // Використовуємо новий об'єкт для категорій
                PopularCategories = GetPopularCategories(),

                // За замовчуванням вибрана категорія "Смартфони"
                SelectedCategory = "Смартфони",

                ActualCategory = new List<string>
                {
                    "Планшети",
                    "Ігрові консолі",
                    "Ноутбуки",
                    "Кондиціонери",
                    "Моноблоки",
                    "Ігрові миші"
                },

                ActualCategories = new List<CategoryModel>
                {
                    new CategoryModel { Id = 1, CategoryIconUrl = "images/actual-category/monoblock.jpg", CategoryName = "Моноблоки" },
                    new CategoryModel { Id = 2, CategoryIconUrl = "images/actual-category/сondocioner.jpg", CategoryName = "Кондиціонери" },
                    new CategoryModel { Id = 3, CategoryIconUrl = "images/actual-category/refreg.png", CategoryName = "Холодильники" },
                    new CategoryModel { Id = 4, CategoryIconUrl = "images/actual-category/soft-toy.png", CategoryName = "М'які іграшки" },
                    new CategoryModel { Id = 5, CategoryIconUrl = "images/actual-category/board-game.png", CategoryName = "Ністільні ігри" },
                    new CategoryModel { Id = 6, CategoryIconUrl = "images/actual-category/car-toy.png", CategoryName = "Іграшкові машини" },
                    new CategoryModel { Id = 7, CategoryIconUrl = "images/actual-category/keyboard.jpg", CategoryName = "Клавіатури для ПК" },
                    new CategoryModel { Id = 8, CategoryIconUrl = "images/actual-category/bisicle.jpg", CategoryName = "Велосипеди" },
                    new CategoryModel { Id = 9, CategoryIconUrl = "images/actual-category/mole-tel.jpg", CategoryName = "Мобільні телефони" },
                    new CategoryModel { Id = 10, CategoryIconUrl = "images/actual-category/tablet.jpg", CategoryName = "Планшети" },
                    new CategoryModel { Id = 11, CategoryIconUrl = "images/actual-category/toy-train.jpg", CategoryName = "Іграшкові залізниці" },
                    new CategoryModel { Id = 12, CategoryIconUrl = "images/actual-category/head-phone.jpg", CategoryName = "навушники" }
                },

                RecommendedVideos = new List<ItemWhithUrlAndPriceModel>
                {
                    new ItemWhithUrlAndPriceModel
                    {
                        ProductDescription = "Найкращий смартфон Samsung до 10000 грн.",
                        ProductPrice = "₴10000",
                        IconUrl = "https://www.youtube.com/embed/KpDiytA1uNM",
                        AvatarUrl = "/images/allo_avatar.jpg"

                    },
                    new ItemWhithUrlAndPriceModel
                    {
                        ProductDescription = "Як правильно вибрати роутер?",
                        ProductPrice = "₴1200",
                        IconUrl = "https://www.youtube.com/embed/q4LcGrxuLtM",
                        AvatarUrl = "/images/allo_avatar.jpg"
                    },
                    new ItemWhithUrlAndPriceModel
                    {
                        ProductDescription = "Як вибрати холодильник e 2025 році",
                        ProductPrice = "₴9000",
                        IconUrl = "https://www.youtube.com/embed/SmNGBOx2688",
                        AvatarUrl = "/images/allo_avatar.jpg"
                    },
                    new ItemWhithUrlAndPriceModel
                    {
                        ProductDescription = "Міні-кондиціонер для дому та офісу",
                        ProductPrice = "₴800",
                        IconUrl = "https://www.youtube.com/embed/bcPs_lB9uc0",
                        AvatarUrl = "/images/tovarydladomy_avatar.jpg"
                    }
                },

                ReviewModels = new List<ReviewModel>
                {
                    new ReviewModel
                    {
                        Image = "https://gagadget.com/media/cache/15/89/15892b54ea78fdc83a301eb41dda9b8b.jpg",
                        Title = "ASUS ExpertBook P5",
                        Link = "https://gagadget.com/uk/asus-expertbook-p5/534695-ogliad-asus-expertbook-p5/",
                        Text = "Ноутбук, який працює більше, ніж ви: огляд ASUS ExpertBook P5"
                    },
                    new ReviewModel
                    {
                        Image = "https://cdn.pixabay.com/photo/2015/01/21/14/14/apple-606761_1280.jpg",
                        Title = "Найкращі ігрові комп'ютери",
                        Link = "https://www.pcgamer.com/best-gaming-pcs/",
                        Text = "Список топових ігрових комп'ютерів 2025 року для максимального задоволення від ігор."
                    },
                    new ReviewModel
                    {
                        Image = "https://air-conditioner.ua/files/global/2024/Korusni-statti/vybraty-kondytsioner-1.jpg",
                        Title = "Як вибрати кондиціонер для дому, квартири, офісу.",
                        Link = "https://air-conditioner.ua/uk/article/yak-vybraty-kondytsioner-pravylno/?srsltid=AfmBOopBEcx5cbRQflIpgXYd7z1-11XaUjBbz8WQDy2AI2K5HqLTycoq",
                        Text = "Рекомендації щодо вибору кондиціонера для різних приміщень і завдань"
                    },
                    new ReviewModel
                    {
                        Image = "https://img.moyo.ua/img/news_desc/1659/165900_1617691982_0.jpg",
                        Title = "Поради по догляду за пральною машиною",
                        Link = "https://www.moyo.ua/ua/news/instrukciya_k_stiralnoyi_mashine_7_sekretov_effektivnogo_ispolzovaniya_tehniki.html?srsltid=AfmBOop7YTX1Z6fnCiPO92xpdx294L_fJAs1pnKJ5Ry8dGYo7mCGkoZx",
                        Text = "Інструкція до пральної машини: 7 секретів ефективного використання техніки"
                    }
                },
            };
        }
    }
}