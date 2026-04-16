const baseUrl = 'http://localhost:5053/api/results/upload'
const form = document.querySelector('form')
const resultParagraph = document.querySelector('p')
const fileListDiv = document.querySelector('.file-list')
const resultsDiv = document.querySelector('.analysis-results')
const jobDescInput = document.querySelector('#job-desc')

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const fileInput = document.querySelector('#file')
    await uploadFile(fileInput)

})

const uploadFile = async (fileInput) => {
    const file = fileInput.files[0]
    if (!file) return;
    const formData = new FormData()
    formData.append("file", file)
    try {
        const response = await fetch(baseUrl, {
            method: "POST",
            body: formData
        })

        if (response.ok) {
            const result = await response.json()
            console.log("Upload successful", result)
            resultParagraph.textContent = `Upload successful: ${JSON.stringify(result)}`;
        }
    } catch (error) {
        console.error("Error uploading:", error);
    }
}
const fetchFiles = async () => {
    try {
        const response = await fetch('http://localhost:5053/api/results/files')
        if(response.ok){
            const files = await response.json()
            fileListDiv.innerHTML = '<h2>Uploaded Files:</h2>'
            files.forEach(file => {
                const fileItem = document.createElement('div')
                fileItem.textContent = file
                fileListDiv.appendChild(fileItem)
                fileItem.addEventListener('click', () => fetchAnalysisResults(file))
            })
        }
    } catch (error) {
        console.error("Error fetching files:", error);
    }
}

const fetchAnalysisResults = async (fileName) => {
    try {
        const response = await fetch(`http://localhost:5053/api/results/extract-text/${fileName}&${encodeURIComponent(jobDescInput.value)}`)
        if(response.ok){
            const result = await response.json()
            matched_skills = result.matched_skills
            missing_skills = result.missing_skills
            score = result.score
            resultsDiv.innerHTML = `<h2>Analysis Results for ${fileName}:</h2>
            <p><strong>Matched Skills:</strong> ${matched_skills.join(', ')}</p>
            <p><strong>Missing Skills:</strong> ${missing_skills.join(', ')}</p>
            <p><strong>Score:</strong> ${score}%</p>
            `

        }
    } catch (error) {
        console.error("Error fetching analysis results:", error);
    }
}

const directoryPath = 'C:/Users/yaraa/Desktop/CvAnalysisTool/Uploads'
fetchFiles()

