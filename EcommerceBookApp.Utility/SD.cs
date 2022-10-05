using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookApp.Utility
{
    public static class SD
    {
        public const string Role_User_Indiv = "Individual";
        public const string Role_User_Comp = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatusPending = "Pending"; //status when the order is created
        public const string StatusAccepted = "Accepted"; //when the payment is approved, then we change the status to approved as well
        public const string StatusInProgress = "Progressing";
        public const string StatusDelivered = "Delivered";
        public const string StatusCancelled = "Cancelled";
        public const string StatusReturned = "Returned";


        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusAccepted = "Approved";
        public const string PaymentStatusDelayedPaymenet = "ApprovedForDelayedPayment"; //30 days to make a payment after order is shipped
        public const string PaymentStatusRejected = "Rejected";

        public const string SessionCart = "SessionShopCart";

        
        
    }
}
