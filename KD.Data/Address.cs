//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KD.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Address
    {
        public int Id { get; set; }
        public int IdDetail { get; set; }
        public string Address1 { get; set; }
        public bool Subscription { get; set; }
    
        public virtual Detail Detail { get; set; }
    }
}
