using System.ComponentModel.DataAnnotations;

namespace Zonosis.Shared.Enumerations
{
    public enum Genero
    {
        //Male
        [Display(Name = "Macho")]
        Masculino,
        //Masculino = 0,
        //Female
        [Display(Name = "Hembra")]
        Femenino
        //Femenino = 1
    }
}
