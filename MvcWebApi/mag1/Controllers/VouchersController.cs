using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Models;

namespace Controllers
{
    public class VouchersController : ApiController
    {
        public HttpResponseMessage GetVouchersList()
        {
            List<Voucher> vouchers = new List<Voucher>();

            vouchers.Add(new Voucher("123456789", "5% discount"));
            vouchers.Add(new Voucher("758413258", "6% discount"));
            vouchers.Add(new Voucher("982112346", "7% discount"));
            vouchers.Add(new Voucher("897541236", "8% discount"));
            vouchers.Add(new Voucher("965412378", "9% discount"));
            return Request.CreateResponse(HttpStatusCode.OK, vouchers, "application/json");
        }
        [System.Web.Mvc.HttpPost]
        public void PostUseVoucher(String voucherCode)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }
    }
}
