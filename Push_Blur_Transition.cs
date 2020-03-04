using ScriptPortal.Vegas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Push_Blur_Transition
{
    enum TransitionMode
    {
        ToLeft,
        ToRight,
        ToTop,
        Down
    }

    enum TransitionVideo
    {
        First,
        Last
    }

    public class PushBlurTransitionModule : ICustomCommandModule
    {
        protected Vegas myVegas = null;

        private int windowWidth = 164;
        private int windowHeight = 600;
        private int buttonHeight = 30;
        private int buttonWidth = 142;
        private int paddingTop = 4;
        private int paddingBottom = 4;
        private int paddingLeft = 4;

        Button fromRightBtn = new Button();
        Button fromLeftBtn = new Button();
        System.Windows.Forms.Label label1 = new System.Windows.Forms.Label();
        TrackBar offsetTrackBar = new TrackBar();
        System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
        System.Windows.Forms.NumericUpDown numericUpDown = new System.Windows.Forms.NumericUpDown();
        System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
        LinkLabel linkLabel = new LinkLabel();

        public void InitializeModule(Vegas vegas)
        {
            myVegas = vegas;
        }

        CustomCommand myViewCommand = new CustomCommand(CommandCategory.Tools, "Push Blur Transition");

        public ICollection GetCustomCommands()
        {
            myViewCommand.DisplayName = Config.DisplayExtensionName;
            myViewCommand.Invoked += this.HandleInvoked;
            myViewCommand.MenuPopup += this.HandleMenuPopup;
            return new CustomCommand[] { myViewCommand };
        }

        void HandleInvoked(Object sender, EventArgs args)
        {
            if (!myVegas.ActivateDockView(Config.DisplayExtensionName))
            {
                DockableControl dockView = new DockableControl(Config.DisplayExtensionName);
                System.Windows.Forms.Label label = new System.Windows.Forms.Label();

                fromRightBtn = new Button();
                fromRightBtn.Location = new System.Drawing.Point(3, 156);
                fromRightBtn.Name = "fromRightBtn";
                fromRightBtn.Size = new System.Drawing.Size(123, 23);
                fromRightBtn.TabIndex = 0;
                fromRightBtn.Text = "From Right";
                fromRightBtn.Click += pushRightClick;
                fromRightBtn.UseVisualStyleBackColor = true;
                dockView.Controls.Add(fromRightBtn);

                fromLeftBtn = new Button();
                fromLeftBtn.Location = new System.Drawing.Point(3, 185);
                fromLeftBtn.Name = "fromLeftBtn";
                fromLeftBtn.Size = new System.Drawing.Size(123, 23);
                fromLeftBtn.TabIndex = 1;
                fromLeftBtn.Text = "From Left";
                fromLeftBtn.Click += pushLeftClick;
                fromLeftBtn.UseVisualStyleBackColor = true;
                dockView.Controls.Add(fromLeftBtn);

                label1 = new System.Windows.Forms.Label();
                label1.AutoSize = true;
                label1.Location = new System.Drawing.Point(4, 137);
                label1.Name = "label1";
                label1.Size = new System.Drawing.Size(49, 13);
                label1.TabIndex = 2;
                label1.Text = "Direction";
                dockView.Controls.Add(label1);

                offsetTrackBar = new TrackBar();
                offsetTrackBar.Location = new System.Drawing.Point(3, 89);
                offsetTrackBar.Name = "trackBar";
                offsetTrackBar.Size = new System.Drawing.Size(123, 45);
                offsetTrackBar.TabIndex = 3;
                offsetTrackBar.Maximum = Config.splitOffsetMaximum;
                offsetTrackBar.Minimum = Config.splitOffsetMinimum;
                offsetTrackBar.Value = Config.splitOffset;
                offsetTrackBar.TickFrequency = 100;
                offsetTrackBar.Scroll += OffsetTrackBar_ValueChanged;
                dockView.Controls.Add(offsetTrackBar);

                numericUpDown = new System.Windows.Forms.NumericUpDown();
                numericUpDown.Location = new System.Drawing.Point(5, 66);
                numericUpDown.Name = "numericUpDown";
                numericUpDown.Size = new System.Drawing.Size(119, 20);
                numericUpDown.TabIndex = 8;
                numericUpDown.Maximum = Config.splitOffsetMaximum;
                numericUpDown.Minimum = Config.splitOffsetMinimum;
                numericUpDown.Value = Config.splitOffset;
                numericUpDown.ValueChanged += NumericUpDown_ValueChanged;
                dockView.Controls.Add(numericUpDown);

                label2 = new System.Windows.Forms.Label();
                label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(4, 50);
                label2.Name = "label2";
                label2.Size = new System.Drawing.Size(75, 13);
                label2.TabIndex = 4;
                label2.Text = "Transition (ms)";
                dockView.Controls.Add(label2);

                label3 = new System.Windows.Forms.Label();
                label3.AutoSize = true;
                label3.Location = new System.Drawing.Point(4, 10);
                label3.Name = "label3";
                label3.Size = new System.Drawing.Size(113, 13);
                label3.TabIndex = 6;
                label3.Text = "PushBlurTransition 0.1";
                dockView.Controls.Add(label3);

                linkLabel = new LinkLabel();
                linkLabel.AutoSize = true;
                linkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                linkLabel.Location = new System.Drawing.Point(3, 254);
                linkLabel.Name = "linkLabel";
                linkLabel.Size = new System.Drawing.Size(108, 12);
                linkLabel.TabIndex = 7;
                linkLabel.TabStop = true;
                linkLabel.Text = "github.com/KLEEEEEER";
                dockView.Controls.Add(linkLabel);

                //dockView.DefaultFloatingSize = new System.Drawing.Size(windowWidth, windowHeight);
                dockView.DefaultFloatingSize = new System.Drawing.Size(145, 308);
                dockView.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                dockView.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);


                /*
                int filledSpace = buttonHeight + paddingTop + paddingBottom;

                offsetTrackBar = new TrackBar();
                offsetTrackBar.Minimum = 50;
                offsetTrackBar.Maximum = 5000;
                offsetTrackBar.Value = 150;
                offsetTrackBar.TickFrequency = 50;
                offsetTrackBar.Width = buttonWidth;
                offsetTrackBar.Height = buttonHeight;
                offsetTrackBar.Location = new System.Drawing.Point(paddingLeft, filledSpace * 1 + paddingTop);
                offsetTrackBar.ValueChanged += OffsetTrackBar_ValueChanged;
                dockView.Controls.Add(offsetTrackBar);

                Button pushLeft = new Button();
                pushLeft.Text = "From Right";
                pushLeft.Click += pushRightClick;
                pushLeft.Location = new System.Drawing.Point(paddingLeft, filledSpace * 2 + paddingTop);
                pushLeft.Width = buttonWidth;
                pushLeft.Height = buttonHeight;
                //pushLeft.Dock = DockStyle.Fill;
                dockView.Controls.Add(pushLeft);

                Button pushRight = new Button();
                pushRight.Text = "From Left";
                pushRight.Click += pushLeftClick;
                pushRight.Location = new System.Drawing.Point(paddingLeft, filledSpace * 3 + paddingTop);
                pushRight.Width = buttonWidth;
                pushRight.Height = buttonHeight;
                dockView.Controls.Add(pushRight);

                //label.Dock = DockStyle.Fill;
                label.Text = "PushBlurExt 0.1";
                label.Location = new System.Drawing.Point(paddingLeft, paddingTop);
                label.Width = buttonWidth;
                label.Height = buttonHeight;
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                dockView.Controls.Add(label);


                dockView.Width = 600;*/

                myVegas.LoadDockView(dockView);
            }
        }

        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            int numericUpDownValue = Convert.ToInt32(numericUpDown.Value);
            Config.splitOffset = numericUpDownValue;
            if (offsetTrackBar.Value != numericUpDownValue)
                offsetTrackBar.Value = numericUpDownValue;
        }

        private void OffsetTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int numericUpDownValue = Convert.ToInt32(numericUpDown.Value);
            Config.splitOffset = offsetTrackBar.Value;
            if (numericUpDown.Value != Convert.ToDecimal(offsetTrackBar.Value))
                numericUpDown.Value = Convert.ToDecimal(offsetTrackBar.Value);
        }

        void HandleMenuPopup(Object sender, EventArgs args)
        {
            myViewCommand.Checked = myVegas.FindDockView(Config.DisplayExtensionName);
        }

        private void pushRightClick(object sender, System.EventArgs e)
        {
            List<TrackEvent> selectedMedias = VegasHelper.FindSelectedEvents(myVegas.Project);
            selectedMedias = selectedMedias.OrderBy((x) => x.Start).ToList();

            if (!validateSelectedTrackEvents(selectedMedias)) return;

            PushBlur pushBlur = new PushBlur(selectedMedias, myVegas, TransitionMode.ToLeft);
            pushBlur.applyEffects();
        }

        private void pushLeftClick(object sender, System.EventArgs e)
        {
            List<TrackEvent> selectedMedias = VegasHelper.FindSelectedEvents(myVegas.Project);
            selectedMedias = selectedMedias.OrderBy((x) => x.Start).ToList();

            if (!validateSelectedTrackEvents(selectedMedias)) return;

            PushBlur pushBlur = new PushBlur(selectedMedias, myVegas, TransitionMode.ToRight);
            pushBlur.applyEffects();
        }

        private bool validateSelectedTrackEvents(List<TrackEvent> selectedMedias)
        {
            if (selectedMedias.Count != 2)
            {
                MessageBox.Show($"Need to select 2 elements. Selected " + selectedMedias.Count + " elements.");
                return false;
            }

            if (VegasHelper.isPickedEventsInTheSameGroup(selectedMedias))
            {
                MessageBox.Show($"Need to select 2 elements from separate groups.");
                return false;
            }

            return true;
        }
    }
}
