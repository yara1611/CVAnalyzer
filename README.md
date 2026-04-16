# CV Analysis Tool
**Version:** 1.0.0-alpha (Proof of Concept)

### ⚠️ Project Status: Prototype / Not Production Ready
This is the **initial version** of the project. It was developed to demonstrate the technical integration between **.NET Core** and **Python-based NLP**. 

**Why it is not production-ready:**
* **Performance:** Currently spawns a new Python process per request; a production version would use a dedicated microservice or message queue.
* **Security:** Minimal file validation and sandbox environment for file uploads.
* **Scalability:** Designed for local development and single-user demonstrations.
* **Environment:** The application is currently configured for Localhost only. It is not yet optimized for deployment to a web server or cloud environment.

## Installation & Setup
**1. Prerequisites**
* .NET SDK
* Python 3.10+

**2. Python Environment Setup**<br/>
Install the necessary packages 
```pip install -r requirements.txt```<br/>
**3. Running the Application**<br/>
Navigate to the project root and run 
```dotnet watch run```<br/>
The application will be available at http://localhost:5053.

## Usage
- Input Job Details: Paste the target job description into the top textarea.
- Upload CV: Choose a .docx file and click upload.
- Analyze: Select any file from the sidebar list to trigger a deep analysis against the job description.

## License
Distributed under the MIT License. See LICENSE for more information. [License](LICENSE)
