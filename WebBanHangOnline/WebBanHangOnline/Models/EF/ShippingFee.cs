﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_ShippingFee")]
    public class ShippingFee
    {
        [Key]
        public int Id { get; set; }
        public decimal Value { get; set; }
    }
}