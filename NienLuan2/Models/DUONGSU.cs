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
    
    public partial class DUONGSU
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DUONGSU()
        {
            this.XETXUs = new HashSet<XETXU>();
        }
    
        public string MA_DuongSu { get; set; }
        public string MA_LoaiDS { get; set; }
        public string CMND { get; set; }
        public string HoTen_DS { get; set; }
        public System.DateTime NamSinh_DS { get; set; }
        public string QueQuan_DS { get; set; }
        public string DiaChi_DS { get; set; }
        public string SoDienThoai_DS { get; set; }
        public bool GioiTinh_DS { get; set; }
    
        public virtual LOAI_DS LOAI_DS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<XETXU> XETXUs { get; set; }
    }
}
