using UnityEngine;
using UnityEngine.InputSystem;

namespace InterdimensionalGroceries.BuildSystem
{
    public class BuildModeInputWrapper
    {
        private InputAction toggleBuildModeAction;
        private InputAction rotateObjectAction;
        private InputAction placeAction;
        private InputAction cancelAction;
        private Keyboard keyboard;
        private Mouse mouse;
        
        public BuildModeInputWrapper()
        {
            keyboard = Keyboard.current;
            mouse = Mouse.current;
            
            toggleBuildModeAction = new InputAction("ToggleBuildMode", binding: "<Keyboard>/e");
            rotateObjectAction = new InputAction("RotateObject", binding: "<Keyboard>/r");
            placeAction = new InputAction("Place", binding: "<Mouse>/leftButton");
            cancelAction = new InputAction("Cancel", binding: "<Keyboard>/escape");
            
            toggleBuildModeAction.AddBinding("<Keyboard>/e");
            rotateObjectAction.AddBinding("<Keyboard>/r");
            placeAction.AddBinding("<Mouse>/leftButton");
            cancelAction.AddCompositeBinding("ButtonWithOneModifier")
                .With("Modifier", "<Mouse>/rightButton");
        }
        
        public void Enable()
        {
            toggleBuildModeAction.Enable();
            rotateObjectAction.Enable();
            placeAction.Enable();
            cancelAction.Enable();
        }
        
        public void Disable()
        {
            toggleBuildModeAction.Disable();
            rotateObjectAction.Disable();
            placeAction.Disable();
            cancelAction.Disable();
        }
        
        public InputAction ToggleBuildMode => toggleBuildModeAction;
        public InputAction RotateObject => rotateObjectAction;
        public InputAction Place => placeAction;
        public InputAction Cancel => cancelAction;
    }
}
