using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace EventControl
{
    class ApplicationControlEvent : IExternalApplication
    {
      
        UpdaterDeleted updater; 

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {

            if (!Directory.Exists(Utills.path))
            {
                Directory.CreateDirectory(Utills.path);
            }
            application.ControlledApplication.DocumentChanged += ControlledApplication_DocumentChanged;

            updater = new UpdaterDeleted(new AddInId(new Guid("023e2e36-adc7-3448-b1e0-d851ac1c40c0")));
            UpdaterRegistry.RegisterUpdater(updater, false);


            List<BuiltInCategory> inCategories = new List<BuiltInCategory>();
            inCategories.Add(BuiltInCategory.OST_Floors);
            inCategories.Add(BuiltInCategory.OST_Walls);
            inCategories.Add(BuiltInCategory.OST_Conduit);
            inCategories.Add(BuiltInCategory.OST_StructuralFraming);
            inCategories.Add(BuiltInCategory.OST_PipeCurves);
            inCategories.Add(BuiltInCategory.OST_PipeFitting);
            inCategories.Add(BuiltInCategory.OST_DuctFitting);
            inCategories.Add(BuiltInCategory.OST_DuctCurves);
            inCategories.Add(BuiltInCategory.OST_CableTray);
            inCategories.Add(BuiltInCategory.OST_CableTrayFitting);
            inCategories.Add(BuiltInCategory.OST_GenericAnnotation);
            inCategories.Add(BuiltInCategory.OST_WallTags);
            inCategories.Add(BuiltInCategory.OST_Windows);
            inCategories.Add(BuiltInCategory.OST_Ceilings);

            ElementMulticategoryFilter multi = new ElementMulticategoryFilter(inCategories);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), multi, Element.GetChangeTypeElementDeletion());


            return Result.Succeeded;
        }

        private void ControlledApplication_DocumentChanged(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        {
            Autodesk.Revit.DB.Document doc = e.GetDocument();
            var app = doc.Application;


            string journalPath = app.RecordingJournalFilename;

            string user = app.Username;
            using (StreamWriter sw = new StreamWriter(Utills.journal))
            {
                sw.WriteLine(journalPath);
                sw.Close();
            }
            using (StreamWriter sw = new StreamWriter(Utills.userPath))
            {
                sw.WriteLine(user);
                sw.Close();
            }
            string versionBuild = app.VersionBuild;
            DateTime d = DateTime.Now;

            List<string> transactions = e.GetTransactionNames().ToList();

            var modified = e.GetModifiedElementIds();
            List<Element> modifiedElements = new List<Element>();
            if (modified?.Count() > 0)
            {
                modifiedElements.AddRange(modified.Select(x => doc.GetElement(x))?.ToList());
            }

            var added = e.GetAddedElementIds();
            List<Element> addedElements = new List<Element>();
            if (added?.Count() > 0)
            {
                addedElements.AddRange(added.Select(x => doc.GetElement(x))?.ToList());
            }

            List<ElementId> DeletedIds = new List<ElementId>();
            var deleted = updater.Deleteds; 
            if (deleted?.Count()>0)
            {
                DeletedIds.AddRange(deleted); 
            }

            Utills.GetDataFromEvents(addedElements, modifiedElements, DeletedIds, transactions, user, out DataTable addtbl, out DataTable edittbl, out DataTable deletedtbl, out DataTable transtbl);
            addtbl.ToTXT(Utills.addPath);
            edittbl.ToTXT(Utills.editPath);
            deletedtbl.ToTXT(Utills.deletedPath);
            transtbl.ToTXT(Utills.transPath);

        }
    }

    public class UpdaterDeleted : IUpdater
    {
        public static AddInId addInId;
        public static UpdaterId updaterId;
        public List<ElementId> Deleteds { get; set; }

        public UpdaterDeleted(AddInId id)
        {
            addInId = id;
            updaterId = new UpdaterId(id, new Guid("9E65445F-DCBB-421D-8904-94B15A3DCB0B"));
        }

        public void Execute(UpdaterData data)
        {
            Deleteds = new List<ElementId>();

            var ids3 = data.GetDeletedElementIds();
           
            var doc = data.GetDocument();


            Deleteds.AddRange(ids3);

        }

        public string GetAdditionalInformation()
        {
            return nameof(UpdaterDeleted);
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        public UpdaterId GetUpdaterId()
        {
            return updaterId;
        }

        public string GetUpdaterName()
        {
            return nameof(UpdaterDeleted);
        }
    }
}
