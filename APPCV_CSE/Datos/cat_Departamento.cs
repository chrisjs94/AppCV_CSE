//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APPCV_CSE.Datos
{
    using System;
    using System.Collections.Generic;
    
    public partial class cat_Departamento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cat_Departamento()
        {
            this.cat_Municipio = new HashSet<cat_Municipio>();
        }
    
        public int id_CatDepartamento { get; set; }
        public string nomDepartamento { get; set; }
        public string descripcion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cat_Municipio> cat_Municipio { get; set; }
    }
}
