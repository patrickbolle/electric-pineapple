//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ElectricPineapple
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        public int messageID { get; set; }
        public int senderID { get; set; }
        public int recieverID { get; set; }
        public string content { get; set; }
        public string readStatus { get; set; }
    
        public virtual CVGSUser CVGSUser { get; set; }
        public virtual CVGSUser CVGSUser1 { get; set; }
    }
}
