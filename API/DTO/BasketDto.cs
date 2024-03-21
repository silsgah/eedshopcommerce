﻿using Entity;

namespace API.DTO
{
    public class BasketDto
    {
        public string ClientId { get; set; }
        public List<BasketItemDto> Items { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
    }
}
