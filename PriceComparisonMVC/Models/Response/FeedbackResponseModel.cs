using System;
using System.Collections.Generic;

namespace PriceComparisonMVC.Models.Response
{

    public class FeedbackResponseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public string FeedbackText { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; }
        public List<string> FeedbackImageUrls { get; set; } = new List<string>();
    }

    public class FeedbacksPagedResponseModel
    {
        public List<FeedbackResponseModel> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    public class FeedbackRequestModel
    {
        public int ProductId { get; set; }
        public string FeedbackText { get; set; }
        public int Rating { get; set; }
        public List<string> Advantages { get; set; } = new List<string>();
        public List<string> Disadvantages { get; set; } = new List<string>();
        public List<string> FeedbackImageUrls { get; set; } = new List<string>();
    }



















    //public class FeedbackResponseModel
    //{
    //    public int Id { get; set; }
    //    public int UserId { get; set; }
    //    public int ProductId { get; set; }
    //    public string FeedbackText { get; set; } = string.Empty;
    //    public DateTime CreatedAt { get; set; }
    //    public int Rating { get; set; }
    //}

}
