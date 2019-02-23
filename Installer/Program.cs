using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.Forms;

// DON'T FORGET to update NuGet package "WixSharp".
// NuGet console: Update-Package WixSharp
// NuGet Manager UI: updates tab

namespace Installer
{
	public class Program
	{
		static void Main()
		{
			var someTool = new Feature("SomeTool", true, true);
			var someOtherTool = new Feature("SomeOtherTool", true, true);
			var mainApp = new Feature("MainApp", true, false);

			var project = new ManagedProject("MyProduct",
				new ElevatedManagedAction(CustomActions.Install, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed, CustomActions.Rollback)
				{
					UsesProperties = "Prop=Install", // need to tunnel properties since ElevatedManagedAction is a deferred action
					RollbackArg = "Prop=Rollback"
				},
				new Binary(new Id("MyCertificateFile"), @"Certificate\certificate.p12"),
				new Certificate("MyCertificate", StoreLocation.localMachine, StoreName.personal, "MyCertificateFile"),
				new Dir("%ProgramFiles%",
					new InstallDir("My Product", new DirFiles(mainApp, @"..\WixSharpPlayground\bin\Debug\*.*", f => !f.EndsWith(".pdb")), new DirPermission("Everyone", GenericPermission.All),
						new Dir("SomeTool", new DirFiles(someTool, @"..\SomeTool\bin\Debug\*.*", f => !f.EndsWith(".pdb")), new DirPermission("Everyone", GenericPermission.All)),
						new Dir("SomeOtherTool", new DirFiles(someOtherTool, @"..\SomeOtherTool\bin\Debug\*.*", f => !f.EndsWith(".pdb")), new DirPermission("Everyone", GenericPermission.All))						
					)));

			project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

			project.ManagedUI = ManagedUI.Empty;    //no standard UI dialogs
			project.ManagedUI = ManagedUI.Default;  //all standard UI dialogs

			//custom set of standard UI dialogs
			project.ManagedUI = new ManagedUI();

			project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
											.Add(Dialogs.Licence)
											.Add(Dialogs.SetupType)
											.Add(Dialogs.Features)
											.Add(Dialogs.InstallDir)
											.Add(Dialogs.Progress)
											.Add(Dialogs.Exit);

			project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
										   .Add(Dialogs.Features)
										   .Add(Dialogs.Progress)
										   .Add(Dialogs.Exit);

			project.Load += Msi_Load;
			project.BeforeInstall += Msi_BeforeInstall;
			project.AfterInstall += Msi_AfterInstall;

			//project.SourceBaseDir = ".";
			//project.OutDir = "<output dir path>";

			

			project.BuildWxs();
			project.BuildMsi();
			Compiler.BuildMsi(project);
		}

		static void Msi_Load(SetupEventArgs e)
		{
			if (!e.IsUISupressed && !e.IsUninstalling)
				MessageBox.Show(e.ToString(), "Load");
		}

		static void Msi_BeforeInstall(SetupEventArgs e)
		{
			if (!e.IsUISupressed && !e.IsUninstalling)
				MessageBox.Show(e.ToString(), "BeforeInstall");
		}

		static void Msi_AfterInstall(SetupEventArgs e)
		{
			if (!e.IsUISupressed && !e.IsUninstalling)
				MessageBox.Show(e.ToString(), "AfterExecute");
		}
	}

	public static class CustomActions
	{
		[CustomAction]
		public static ActionResult Install(Session session)
		{
			MessageBox.Show(session.Property("Prop"), "Install");

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult Rollback(Session session)
		{
			MessageBox.Show(session.Property("Prop"), "Rollback");

			return ActionResult.Success;
		}
	}
}