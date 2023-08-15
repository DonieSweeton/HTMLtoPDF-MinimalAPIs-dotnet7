using HtmltoPDF.Data;
using HtmltoPDF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.Endpoints
{
    public static class HtmlToPdfApi
    {
        public static void HtmlToPdfEndpoint(this WebApplication app)
        {
            app.MapGet("/html-to-pdf/{groupId:int}", async (HttpContext httpContext, int groupId) =>
            {
                // Retrieve the AppDbContext from the request services.
                var dbContext = httpContext.RequestServices.GetRequiredService<AppDbContext>();

                // Retrieve the group and users data from the database.
                var group = await dbContext.Groups.FindAsync(groupId);
                var usersInGroup = await dbContext.Users.Where(u => u.GroupId == groupId).ToListAsync();

                if (group == null)
                {
                    // If the group with the given groupId doesn't exist, return NotFound.
                    return Results.NotFound();
                }

                // Generate the dynamic HTML content using the retrieved group and users data.
                string htmlContent = GenerateHtmlContent(group, usersInGroup);

                // Instantiate Renderer
                var renderer = new ChromePdfRenderer();

                // Create a new PDF document using IronPDF
                var pdfDocument = renderer.RenderHtmlAsPdf(htmlContent);

                // Return PDF as response
                var pdfData = pdfDocument.BinaryData;
                return Results.File(pdfData, "application/pdf", "file.pdf");
            });
        }

        private static string GenerateHtmlContent(Group group, List<User> users)
        {
            // TODO: Create the dynamic HTML content using the retrieved group and users data.
            // You can use string interpolation or a templating library to insert the data into the HTML.

            // For example:
            string imageUrl = "https://images.pexels.com/photos/15379/pexels-photo.jpg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"; // Replace this with the actual URL of the image

            string htmlContent = $@"
                    <html>
                    <head>
                        <title>Group Report</title>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css'>
                    </head>
                    <body>
                        <div class='container'>
                            <h1 class='mt-4'>Group Report: {group.GroupName}</h1>
                            <p>Date: {DateTime.UtcNow:yyyy-MM-dd}</p>
                            <p>Group Owner: {group.CreatedBy}</p>

                            <h4>Users:</h4>
                            <ul class='list-group'>
                                {string.Join("", users.Select(u => $"<li class='list-group-item'>{u.UserName} - {u.UserEmail}</li>"))}
                            </ul>

                            <img src='{imageUrl}' alt='Group Image' class='img-fluid mt-4'> <!-- Add the image here -->
                        </div>
                    </body>
                    </html>";


            return htmlContent;
        }
    }
}
