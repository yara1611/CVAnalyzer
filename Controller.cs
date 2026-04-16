using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Diagnostics;
using System.Text.Json;

[Controller]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    [HttpGet]
    public IActionResult test()
    {
        return Ok("API Works");
    }

    [HttpGet("Response")]
    public IActionResult GetResult()
    {
        Result result = new Result
        {
            Name = "John Doe",
            Skills = new List<string> { "C#", "ASP.NET", "SQL" },
            experienceYears = 5,
            matchScore = 85.5
        };
        return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }
        string filePath = Path.Combine(uploadsFolder, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return Ok(new { name = file.FileName, size = file.Length });
    }

    [HttpGet("extract-text/{fileName}&{jobDescription}")]
    public IActionResult ExtractTextFromDocx(string fileName, string jobDescription)
    {
        string rawResult = CallPythonScript("Uploads/" + fileName, jobDescription);
        if(string.IsNullOrEmpty(rawResult))
        {
            return BadRequest("Failed to extract text from the document.");
        }
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };  
            var analysis = JsonSerializer.Deserialize<AnalysisResult>(rawResult,options);
                if (analysis == null)
                {
                    return BadRequest("Failed to parse analysis result.");
                }
                return Ok(analysis);
        }
        catch (JsonException e)
        {
            
           return StatusCode(500, "Failed to parse Python output as JSON: " + rawResult + " Error: " + e.Message);
        }
        
    }
    
    [HttpGet("files")]
    public List<string> UploadedFiles()
    {
        string uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploadsDirectory))
        {
            return new List<string>();
        }
        return Directory.GetFiles(uploadsDirectory).Select(Path.GetFileName).ToList();
    }
    public string CallPythonScript(string filePath, string jobDescription)
    {
        var start = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = $"Scripts/script.py \"{filePath}\" \"{jobDescription}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        using (var process = Process.Start(start))
        {
            using (var reader = process.StandardOutput)
            {
                return reader.ReadToEnd();
            }
        }
    }
}

public class Result
{
    public string Name { set; get; }
    public List<string> Skills { get; set; }
    public int experienceYears { get; set; }
    public double matchScore { get; set; }

}

public class AnalysisResult
{
    public List<string> Matched_skills { get; set; } = new();
    public List<string> Missing_skills { get; set; } = new();
    public double Score { get; set; }
}