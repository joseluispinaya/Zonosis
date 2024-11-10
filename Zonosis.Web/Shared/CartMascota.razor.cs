using Microsoft.AspNetCore.Components;
using MudBlazor;
using Zonosis.Shared.Entities;

namespace Zonosis.Web.Shared
{
    public partial class CartMascota
    {
        private bool arrows = true;
        private bool bullets = true;
        private bool enableSwipeGesture = true;
        private bool autocycle = true;
        private Transition transition = Transition.Slide;

        [Parameter, EditorRequired] public List<Pet> Pets { get; set; } = null!;
    }
}