namespace Botwin.Samples
{
    using System.IO;
    using Botwin.ModelBinding;
    using Botwin.Request;
    using Botwin.Response;

    public class ActorsModule : BotwinModule
    {
        public ActorsModule(IActorProvider actorProvider)
        {
            this.Get("/actors", async (req, res, routeData) =>
            {
                var people = actorProvider.Get();
                await res.AsJson(people);
            });

            this.Get("/actors/{id:int}", async (req, res, routeData) =>
            {
                var person = actorProvider.Get(routeData.As<int>("id"));
                await res.Negotiate(person);
            });

            this.Put("/actors/{id:int}", async (req, res, routeData) =>
            {
                var result = req.BindAndValidate<Actor>();

                if (!result.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                //Update the user in your database

                res.StatusCode = 204;
            });

            this.Post("/actors", async (req, res, routeData) =>
            {
                var result = req.BindAndValidate<Actor>();

                if (!result.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                //Save the user in your database

                res.StatusCode = 201;
                await res.Negotiate(result.Data);
            });

            this.Get("/actors/download", async (request, response, routeData) =>
            {
                using (var video = new FileStream("earth.mp4", FileMode.Open))
                {
                    await response.FromStream(video, "video/mp4");
                }
            });
        }
    }
}
