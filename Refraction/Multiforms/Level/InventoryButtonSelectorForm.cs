using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine.Multiforms;

namespace Refraction_V2.Multiforms.Level
{
    public class InventoryButtonSelectorForm : Form
    {

        InventoryForm Inventory;

        public InventoryButtonSelectorForm(InventoryForm inventory)
            : base(true)
        {
            Inventory = inventory;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            var index = Inventory.CurrentlySelectedTile;
        }

    }
}
