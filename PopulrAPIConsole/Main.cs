using System;
using Populr;
using System.IO;
using System.Collections.Generic;

namespace PopulrAPIConsole
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// Instantiate the populr api, with the optional host parameter set for localhost
			PopulrAPI populr = new PopulrAPI ("UXXMOCJW-BKSLPCFI-UQAQFWLO", "http://api.lvh.me:3000");

			// Create a new pop from a template
			List<Template> templates = populr.Templates().All();
			Console.WriteLine ("Found " + templates.Count + " templates");
			Pop newPop1 = new Pop(templates[0]);
			Pop newPop2 = templates[0].Pops().Build();

			newPop1.title = "Wow a new pop!";
			newPop1.slug = "wowzers";

			// Populate regions that are empty
			if (newPop1.HasUnpopulatedTag("first_name"))
				newPop1.PopulateTag("first_name", "Ben Gotow");

			if (newPop1.HasUnpopulatedRegion("hire_documents")) {
				FileStream stream = new FileStream("/Bible-Quote-93.jpg", FileMode.Open);
				Asset documentAsset = (Asset)populr.Documents().Build(stream, "My Profile Picture").Save();
				newPop1.PopulateRegion("hire_documents", documentAsset);
				stream.Close ();
			}

			newPop1.Save ();
			Console.WriteLine("Created new pop from template 0: " + newPop1._id);
			newPop1.Publish();
			Console.WriteLine("The pop is published at " + newPop1.published_pop_url);
		}
	}
}
