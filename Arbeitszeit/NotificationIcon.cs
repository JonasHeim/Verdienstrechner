/*
 * Created by SharpDevelop.
 * User: JHeim
 * Date: 17.02.2017
 * Time: 12:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using System.IO;
using LINQtoCSV;
using System.Collections.Generic;


namespace Arbeitszeit
{
	public class NotificationIcon
	{
		private NotifyIcon notifyIcon;
		private ContextMenu notificationMenu;
		protected Einstellungen FormSettings = new Einstellungen();
		private About FormAbout = new About();
		private float total_earning = 0;
		private decimal hourly_wage = 0;
		
		/* CSV save file, name depends on month and year */
		private static String file_save_Time = "Stempelzeiten_"+ DateTime.Now.Month +"_"+ DateTime.Now.Year +".csv";
		
		protected static List<Stempelzeit> eintraegeCSV = new List<Stempelzeit>();
				
		protected static CsvFileDescription outputCSV = new CsvFileDescription{
			SeparatorChar = ';', FirstLineHasColumnNames = false, FileCultureName = "de-DE", EnforceCsvColumnAttribute = true
		};
  
		protected static CsvFileDescription inputCSV = new CsvFileDescription{
			SeparatorChar = ';', FirstLineHasColumnNames = false, EnforceCsvColumnAttribute = true
		};
	
		
		protected static CsvContext cc = new CsvContext();
		protected static Stempelzeit heutiger_eintrag = new Stempelzeit();
		protected static IEnumerable<Stempelzeit> readStempelzeit;
		protected static Stempelzeit letzter_eintrag;
		
		protected static System.Threading.Timer update_timer;
		
		#region Initialize icon and menu
		public NotificationIcon()
		{
			notifyIcon = new NotifyIcon();
			notificationMenu = new ContextMenu(InitializeMenu());
			notifyIcon.Icon =  Arbeitszeit.Resources.arbeitszeit;
			notifyIcon.ContextMenu = notificationMenu;
			notifyIcon.BalloonTipTitle = "Verdienst heute";
			notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
			
			//Register the new MouseMove-Event
			notifyIcon.Click += new EventHandler(notifyIcon_Click);	
			
			//TODO
			//Register the new Shutdown-Event to method shutdownHandler
			
			
			
		}
		
		/* is called when clicked on icon */
		private void notifyIcon_Click(object Sender, EventArgs e){
			
			//Calculate earnings and print the info
			TimeSpan time_span = TimeSpan.FromMilliseconds(System.Environment.TickCount);
			total_earning = (float)((time_span.Hours)+((float)time_span.Minutes/60))*(float)FormSettings.getStundenlohn();
			
			System.Diagnostics.Debug.WriteLine( "timespan: "+time_span.ToString());
			System.Diagnostics.Debug.WriteLine( "total_earning: "+total_earning);
			
			if(time_span.Hours >= 6 && time_span.Hours < 9){
				System.Diagnostics.Debug.WriteLine( ">=6 && < 9");
				/* minus half an hour pause */
				total_earning = total_earning - ((float)FormSettings.getStundenlohn() * 0.5f);
				
				/* calculate the earning */
				//this.notifyIcon.BalloonTipText = "PC läuft seit ~ "+time_span.Hours.ToString()+":"+time_span.Minutes+"\nAbzüglich Pause 30 Minuten."+"\nVerdient ~ "+total_earning+"€";
				this.notifyIcon.BalloonTipText = string.Format("PC läuft seit ~ {0:00}:{1:00}\nAbzüglich Pause 30 Minuten.\nVerdient ~{2:0}€", time_span.Hours, time_span.Minutes, total_earning);
				this.notifyIcon.ShowBalloonTip(5000);
			}
			else if(time_span.Hours >= 9 && time_span.Minutes >= 15){
				System.Diagnostics.Debug.WriteLine( ">= 9 && minutes >= 15");
				/* minus 15min pause*/
				total_earning = total_earning - ((float)FormSettings.getStundenlohn() * 0.75f);
				
				/* calculate the earning */
				//this.notifyIcon.BalloonTipText = "PC läuft seit ~ "+time_span.Hours.ToString()+":"+time_span.Minutes+"\nAbzüglich Pause 45 Minuten."+"\nVerdient ~ "+total_earning+"€";
				this.notifyIcon.BalloonTipText = string.Format("PC läuft seit ~ {0:00}:{1:00}\nAbzüglich Pause 45 Minuten.\nVerdient ~{2:00}€", time_span.Hours, time_span.Minutes, total_earning);
				this.notifyIcon.ShowBalloonTip(5000);
			}
			else{
				System.Diagnostics.Debug.WriteLine( "else");
				/* calculate the earning */
				//this.notifyIcon.BalloonTipText = "PC läuft seit ~ "+time_span.Hours.ToString()+":"+time_span.Minutes+"\nVerdient ~ "+total_earning+"€";
				this.notifyIcon.BalloonTipText = string.Format("PC läuft seit ~ {0:00}:{1:00}\nVerdient ~{2:00}€", time_span.Hours, time_span.Minutes, total_earning);
				this.notifyIcon.ShowBalloonTip(5000);
			}
			
		}
		
		private MenuItem[] InitializeMenu()
		{
			MenuItem[] menu = new MenuItem[] {
				new MenuItem("Einstellungen", menuSettingsClick),
				new MenuItem("Über", menuAboutClick),
				new MenuItem("Schliessen", menuExitClick)
			};
			return menu;
		}
		#endregion
		
		#region Main - Program entry point
		/// <summary>Program entry point.</summary>
		/// <param name="args">Command Line Arguments</param>
		[STAThread]
		public static void Main(string[] args)
		{
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			//test if *.csv save file is present
			if(System.IO.File.Exists(file_save_Time) == true){
				//File exists
				Console.WriteLine(file_save_Time+" vorhanden.");
				
				//TODO	

				//Check if PC was boote previous this day
				//If false, create data set for this day and add entry for start time
				//If true, go on				
				
				
				
				//read file and add to list
				readStempelzeit = cc.Read<Stempelzeit>(file_save_Time, inputCSV);
				eintraegeCSV = new List<Stempelzeit>(readStempelzeit);
				if(eintraegeCSV.Count != 0){
					letzter_eintrag = eintraegeCSV.ElementAt(eintraegeCSV.Count-1);
				}
				else{
					letzter_eintrag = new Stempelzeit();
				}
				if(letzter_eintrag.BootTime.Date == DateTime.Now.Date){
					
					//Eintrag fuer heute bereits vorhanden
					System.Diagnostics.Debug.WriteLine("Eintrag fuer heute bereits vorhanden");
				}
				else{
					
					//Kein Eintrag für heute vorhanden; erzeugen...
					System.Diagnostics.Debug.WriteLine("Eintrag fuer heute noch nicht vorhanden");
					
					//Eintrag erzeugen
					heutiger_eintrag.BootTime = DateTime.Now;
					heutiger_eintrag.ShutdownTime = DateTime.Now;
					
					eintraegeCSV.Add(heutiger_eintrag);
					
					//In Datei schreiben
					cc.Write(eintraegeCSV, file_save_Time, outputCSV);
				}
				
				//initialize and start timer to call updateCSV() every minute
				update_timer = new System.Threading.Timer(e => updateCSV(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
				
							
			}
			else{
				//Create file
				try{
					
					FileStream fs = File.Create(file_save_Time);
					
				}
				catch(Exception e){
					Console.WriteLine(e.ToString());
				}
				
			}
			
			bool isFirstInstance;
			// Please use a unique name for the mutex to prevent conflicts with other programs
			using (Mutex mtx = new Mutex(true, "Arbeitszeit", out isFirstInstance)) {
				if (isFirstInstance) {
										
					if(args.Length == 1){
							
						NotificationIcon notificationIcon = new NotificationIcon();
						notificationIcon.notifyIcon.Visible = true;					
						
						notificationIcon.hourly_wage = decimal.Parse(args[0]);
						notificationIcon.FormSettings.setStundenlohn(notificationIcon.hourly_wage);
						
						Application.Run();
						notificationIcon.notifyIcon.Dispose();
					}
					else{
						MessageBox.Show("Aufrufargument[0] fuer Stundenlohn benötigt.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error
					               );
						isFirstInstance = false;
					}
							
					
					
				} else {
					// The application is already running
					MessageBox.Show("Anwendung läuft bereits.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error
					               );				}
			} // releases the Mutex
		}
		#endregion
		
		#region Event Handlers
		private void menuAboutClick(object sender, EventArgs e)
		{
			FormAbout.Show();
		}
		
		private void menuSettingsClick(object sender, EventArgs e)
		{
			FormSettings.Show();
		}
		
		private void menuExitClick(object sender, EventArgs e)
		{
			Application.Exit();
		}
		
		//is called when PC shuts down
		private void shutdownHandler(object sender, EventArgs e){
			
			//TODO
			//open *.csv save file
			//check for entry for today
			//check if data set for today has got a stop entry
			//if true, check if entry is previous
				//if true, overwrite entry with current time
				//if not, warning (inconsitstency?!?)
			//if not, add stop entry
			
		}
		#endregion
		
		#region Utility
		
		private static void updateCSV(){
			
			//test if *.csv save file is present
			if(System.IO.File.Exists(file_save_Time) == false){
				
				//create file
				try{
					
					FileStream fs = File.Create(file_save_Time);
					
				}
				catch(Exception e){
					Console.WriteLine(e.ToString());
				}
					
		
			}
			
			//read file and add to list
			readStempelzeit = cc.Read<Stempelzeit>(file_save_Time, inputCSV);
			eintraegeCSV = new List<Stempelzeit>(readStempelzeit);
			
			letzter_eintrag = eintraegeCSV.ElementAt(eintraegeCSV.Count-1);
			if(letzter_eintrag.BootTime.Date == DateTime.Now.Date){
				
				//Eintrag fuer heute bereits vorhanden
				System.Diagnostics.Debug.WriteLine("Update Eintrag fuer heute.");
				
				
				//Update
				letzter_eintrag.ShutdownTime = DateTime.Now;
				
				//In Datei schreiben
				cc.Write(eintraegeCSV, file_save_Time, outputCSV);
							
				
			}
			
		}
		
		#endregion
		
	}

	//Objekte für Datensätze in .csv Datei
	public class Stempelzeit{
		
		    [CsvColumn(Name = "BootTime", FieldIndex = 1)]
		    public DateTime BootTime { get; set; }
		
		    [CsvColumn(Name = "ShutdownTime", FieldIndex = 2)]
		    public DateTime ShutdownTime { get; set; }		
				
	}
}
