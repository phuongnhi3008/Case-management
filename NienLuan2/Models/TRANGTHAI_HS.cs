//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NienLuan2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TRANGTHAI_HS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRANGTHAI_HS()
        {
            this.HOSO_VUAN = new HashSet<HOSO_VUAN>();
        }
    
        public string MA_TrangThai { get; set; }
        public string Ten_TT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSO_VUAN> HOSO_VUAN { get; set; }
    }
}
