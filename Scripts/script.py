import re
import json
import docx
import sys


##READING DOCX FILE
def read_docx(file_path):
    doc = docx.Document(file_path)
    text = "\n".join([para.text for para in doc.paragraphs])
    return text

file_path = sys.argv[1]
job_text = sys.argv[2]
text=read_docx(file_path)


##print(repr(text))

##CLEANING THE TEXT
def clean_text(text):
    ##Lowercase the text to ensure case-insensitive searching
    text = text.lower()
    # replace separators with space
    text = re.sub(r"[\/,()\-]", " ", text)
    # remove unwanted characters but keep + # .
    text = re.sub(r"[^a-z0-9+#. ]", " ", text)
    # normalize spaces
    text = " ".join(text.split())
    return text

text=clean_text(text)
job_text=clean_text(job_text)

##Test Skill Detection
##print("python" in text)

##EXTRACTING SKILLS
with open('Scripts\data.json', 'r') as file:
    skill_aliases = json.load(file)

##SCRAPING TO ADD MORE SKILLS TO THE JSON FILE LATER

def extract_skills(text, skill_aliases):
    found_skills=[]
    for skill,variantions in skill_aliases.items():
        for v in variantions:
            if f" {v} " in f" {text} ":
                found_skills.append(skill)
                break
    return found_skills
cv_skills = extract_skills(text, skill_aliases)
job_skills = extract_skills(job_text, skill_aliases)
job_skills=[s.lower() for s in job_skills]


def compare_skills(cv_skills, job_skills):
    cv_skills_set = set(cv_skills)
    job_skills_set = set(job_skills)
    
    matched_skills = cv_skills_set.intersection(job_skills_set)
    missing_skills = job_skills_set.difference(cv_skills_set)
    
    score = len(matched_skills) / len(job_skills_set) * 100 if job_skills_set else 0
    
    return {
        "matched_skills": list(matched_skills),
        "missing_skills": list(missing_skills),
        "score": round(score, 2)
    }


#RETURN RESULTS AS JSON
print(json.dumps(compare_skills(cv_skills, job_skills)))
