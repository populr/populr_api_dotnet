# Populr.me API .NET


## Setup

We've created a wrapper for the Populr API that makes it easy to use in .NET environments. To get started, take a look at the files in the Populr folder. You can add this project to your workspace as a dependency, or integrate the files directly into your project. The source makes extensive use of RestSharp, an open-source solution for REST service interfaces in .NET. 

RestSharp is included as a submodule. To pull it into the project after you pull from git, run `git submodule update`. You may have to remove the RestSharp project reference and add it again.



The PopulrAPIConsole project shows an example use of the API. The important parts of the sample are explained below.


## Performing Common Tasks

### Initialization

      PopulrAPI populr = new PopulrAPI ("LIIZAOPK-ITFEAWEB-HJYCKMPQ");


### Listing Available Templates

      // fetch all available templates
      List<Template> templates = populr.Templates().All();
      
      // or iterate over the collection
      foreach (Template template in populr.Templates())
      	  Console.WriteLine(template._id);
      
      // or fetch a specific range (offset, count)
      List<Template> page2 = populr.Templates().Range(20, 10);
      
      Console.WriteLine ("Found " + templates.Count + " templates");


### Uploading an Asset

      FileStream stream = new FileStream("/my_picture.jpg", FileMode.Open);
	  Asset image = (Asset)populr.Images().Build(stream, "My Profile Picture").Save();
      stream.Close ();


### Creating a Pop based on a Template

      // Note: All pops must be created based on an existing template in your Populr account.
      Pop pop = new Pop(templates[0]);
      
      // Or, create one ActiveRecord-style!
      Template template = populr.Templates().First();
      Pop pop = template.Pops().Build();

### Filling a Pop with Content

      // Basic attributes can be modified directly.
      pop.title = "Wow a new pop!";
      pop.slug = "new-pop";
      
      // Always check to make sure regions and tags exist before populating them.
      // Trying to populate a region that does not exist will result in an
      // APIException being thrown.
      if (pop.HasUnpopulatedTag("personal_site"))
        pop.PopulateTag("personal_site", "http://www.gotow.net/");

      if (pop.HasUnpopulatedRegion("profile_image_region")) {
        pop.PopulateRegion("profile_image_region", imageAsset);
      }

      // Synchronously saves changes. An APIException will be thrown if validation fails.
      // For example, if you've assigned the pop the same slug as another pop.
      pop.Save ();

### Publishing a Pop

      // Publishes the pop immediately. After calling this method, the pop's published_pop_url
      // is set populated.
      pop.Publish();
      
      // Print the published pop URL
      Console.WriteLine("The pop is published at " + pop.published_pop_url);

### Creating Tracers

	  Pop pop = populr.Pops().Find("51782e69dd02dc70ed00000d");
 	  Tracer tracer = pop.Tracers().Build();
   	  tracer.name = "Tracer for bengotow@gmail.com";
	  tracer.Save();
	  
      // Print the published pop URL
      Console.WriteLine("The traced pop URL is" + pop.published_pop_url + "?" + tracer.code);


### Tracer Analytics

	  Console.WriteLine(tracer.Views() + " views");
	  Console.WriteLine(tracer.Clicks() + " clicks");
	  Console.WriteLine(tracer.ClicksForLink("http://www.apple.com/") + "on the http://www.apple.com/ link"");
	  Console.WriteLine(tracer.ClicksForRegion("profile_image_region") + " clicks on the profile image");


## Development

The C# code was authored in [MonoDevelop](http://monodevelop.com/), a Mac app similar to Microsoft Visual Studio. MonoDevelop is free and cross platform.

## We welcome pull requests!
