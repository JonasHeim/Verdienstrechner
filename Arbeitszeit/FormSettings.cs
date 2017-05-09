/*
 * Created by SharpDevelop.
 * User: JHeim
 * Date: 20.02.2017
 * Time: 14:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;

namespace Arbeitszeit
{
	/// <summary>
	/// Description of FormSettings.
	/// </summary>
	public partial class Einstellungen : Form
	{
		private decimal stundenlohn;
		
		public Einstellungen()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			stundenlohn = numericUpDown1.Value;
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button1Click(object sender, EventArgs e)
		{
			setStundenlohn(numericUpDown1.Value);
			this.Hide();
		}
		void Button2Click(object sender, EventArgs e)
		{
			this.Hide();
		}
		
		public decimal getStundenlohn(){
			return stundenlohn;
		}
		
		public void setStundenlohn(decimal new_stundenlohn){
			this.stundenlohn = new_stundenlohn;
			this.numericUpDown1.Value = new_stundenlohn;
		}
	}
}
