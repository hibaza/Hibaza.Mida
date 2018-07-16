using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralRepository _referralRepository;
        private readonly IMessageService _messageService;
        private readonly ICustomerService _userService;
        public ReferralService(IReferralRepository referralRepository, IMessageService messageService, ICustomerService userService)
        {
            _referralRepository = referralRepository;
            _messageService = messageService;
            _userService = userService;
        }

        public void Create(Referral referral)
        {
            _referralRepository.Upsert(referral);
        }

        public int UpdateCustomerId()
        {
            return _referralRepository.UpdateCustomerId();
        }

        public Referral CreateReferral(string business_id, Thread threads, string referral_id, string sender_ext_id, string recipient_ext_id, string data,string thread_id)
        {
            string[] ps = data.Split(',');
            var type = ps.Length > 0 ? ps[0] : "";
            var sku = ps.Length > 1 ? ps[1] : "";
            var pid = ps.Length > 2 ? ps[2] : "";
            var url = ps.Length > 3 ? ps[3] : "";
            if (ps.Length > 4) {
                url = data.Substring(type.Length + sku.Length + pid.Length + 3);
            }

            Referral referral = new Referral
            {
                business_id = business_id,
                thread_id = thread_id,
                customer_id = threads ==null ? null: threads.customer_id,
                sender_ext_id = sender_ext_id,
                recipient_ext_id = recipient_ext_id,
                data = data,
                product_sku = sku,
                product_id = pid,
                product_url = url,
                type = type,
                timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow)
            };

            referral.id = string.IsNullOrWhiteSpace(referral_id) ? Core.Helpers.CommonHelper.DateTimeToTimestamp13Digits(referral.created_time).ToString() : referral_id;
            _referralRepository.Upsert(referral);
            return referral;
        }

        public async Task<IEnumerable<Referral>> GetReferralsByCustomer(string business_id, string customer_id, Paging page)
        {
            return await _referralRepository.GetReferralsByCustomer(business_id, customer_id, page);
        }


        public async Task<IEnumerable<Referral>> GetReferrals(string business_id, string thread_id, Paging page)
        {
            return await _referralRepository.GetReferrals(business_id, thread_id, page);
        }

        public IEnumerable<Referral> GetAll(string business_id, Paging page)
        {
            return _referralRepository.GetAll(business_id, page).Result;
        }

        public Referral GetById(string business_id, string id)
        {
            return _referralRepository.GetById(business_id, id);
        }

        public async Task<IEnumerable<Referral>> GetReferralsByCustomerIsNull(int limit)
        {
            return await _referralRepository.GetReferralsByCustomerIsNull(limit);
        }
    }
}
