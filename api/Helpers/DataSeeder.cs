using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Enums;
using api.Interfaces;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Identity;

namespace api.Helpers
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(string adminPassword, UserManager<AppUser> userManager)
        {
            var adminEmail = "appAdmin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    PhoneNumber = "+1234567890"
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(admin, "ADMIN");
                    if (!roleResult.Succeeded) throw new Exception($"Failed to add user to role ADMIN. Errors: {string.Join(", ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
                else
                {
                    throw new Exception($"Failed to create user. Errors: {string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }
        }

        public static async Task<bool> SeedCompanyDataAsync(
     UserManager<AppUser> userManager,
     ICompanyRepository companyRepository,
     ApplicationDbContext dbContext)
        {
            var companyData = GetMockCompanyData();
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var (appUser, company, password) in companyData)
                {
                    var existingUser = await userManager.FindByEmailAsync(appUser.Email);
                    if (existingUser == null)
                    {
                        var result = await userManager.CreateAsync(appUser, password);
                        if (!result.Succeeded)
                            throw new Exception($"Failed to create user {appUser.Email}. Errors: {string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");

                        existingUser = appUser;

                        var roleResult = await userManager.AddToRoleAsync(existingUser, "COMPANY");
                        if (!roleResult.Succeeded)
                            throw new Exception($"Failed to add user {existingUser.Email} to role COMPANY. Errors: {string.Join(", ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                    }

                    var existingCompany = await dbContext.Companies.FindAsync(company.Id);
                    if (existingCompany == null)
                    {
                        company.AppUserId = existingUser.Id;
                        await companyRepository.CreateAsync(company);
                    }
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to add company data. Error: {ex.Message}", ex);
            }
        }

        private static List<(AppUser, Company, string)> GetMockCompanyData()
        {
            var data = new List<(AppUser, Company, string)>();

            // ---------- Company 1: GlobalTech ----------
            var appUser1 = new AppUser
            {
                Id = "30e67072-67cd-4a8f-a798-2e83d32a96e3",
                UserName = "GlobalTech",
                Email = "info@globaltech.com",
                PhoneNumber = "+1-555-123-4567"
            };

            var company1 = new Company
            {
                Id = Guid.Parse("b8f1d9a1-9bb8-47b6-be20-93f20cf5a73a"),
                AppUserId = appUser1.Id,
                AppUser = appUser1,
                SelfDescription = "GlobalTech is a pioneering technology firm reimagining the digital future.",
                Location = "San Francisco, CA"
            };

            company1.Vacancies = new List<Vacancy>
    {
        new Vacancy
        {
            Id = Guid.Parse("a3f98c7e-74b3-49eb-9e4d-93e4b80289fa"),
            CompanyId = company1.Id,
            Company = company1,
            Title = "Software Engineer",
            Description = "GlobalTech seeks an innovative Software Engineer to develop cutting-edge applications using C# and .NET. In this role, you will design, build, and maintain scalable cloud-based solutions leveraging microservices architecture and agile methods.",
            CandidateDescription = "Candidates should bring proven expertise in C#, .NET, and modern cloud services with excellent collaboration skills.",
            Position = "Backend Software Specialist",
            SalaryMin = 90000f,
            SalaryMax = 130000f,
            WorkMode = VacancyWorkModes.REMOTE,
            LivingConditions = "Modern workspace with flexible remote options and dynamic team collaboration."
        },
        new Vacancy
        {
            Id = Guid.Parse("7f2d3c11-8d85-4eba-b800-3a877a680d45"),
            CompanyId = company1.Id,
            Company = company1,
            Title = "Cloud Architect",
            Description = "GlobalTech seeks a Cloud Architect to lead the design and deployment of advanced cloud infrastructures. With a focus on platforms like Azure and AWS, you will ensure scalability, security, and high performance across enterprise systems.",
            CandidateDescription = "Applicants must have hands-on experience in cloud architecture and enterprise-level infrastructure design.",
            Position = "Enterprise Cloud Strategist",
            SalaryMin = 120000f,
            SalaryMax = 160000f,
            WorkMode = VacancyWorkModes.OTHER,
            LivingConditions = "An innovative environment offering both remote flexibility and office collaboration."
        },
        new Vacancy
        {
            Id = Guid.Parse("ce5bf91d-5b32-4995-8dfc-f2cab4a0f1d3"),
            CompanyId = company1.Id,
            Company = company1,
            Title = "DevOps Engineer",
            Description = "GlobalTech is hiring a DevOps Engineer to automate deployment pipelines and enhance continuous integration practices. This role focuses on container orchestration, infrastructure-as-code strategies, and overall system automation for efficient software delivery.",
            CandidateDescription = "Candidates should be experienced with Docker, Kubernetes, and CI/CD pipelines along with strong scripting abilities.",
            Position = "Automation & Deployment Engineer",
            SalaryMin = 85000f,
            SalaryMax = 125000f,
            WorkMode = VacancyWorkModes.REMOTE,
            LivingConditions = "A hybrid setup that combines the benefits of remote work with modern office spaces."
        }
    };

            string password1 = "GloTech@1234";
            data.Add((appUser1, company1, password1));

            // ---------- Company 2: InnovateX Systems ----------
            var appUser2 = new AppUser
            {
                Id = "d4c437e1-2f68-4f9b-9c54-0b3c76b6a12f",
                UserName = "InnovateXSystems",
                Email = "info@innovatex.com",
                PhoneNumber = "+1-555-234-5678"
            };

            var company2 = new Company
            {
                Id = Guid.Parse("9c5b11e1-f2d4-4def-91aa-12b6c9b8f3c7"),
                AppUserId = appUser2.Id,
                AppUser = appUser2,
                SelfDescription = "At InnovateX Systems, innovation meets excellence in developing state-of-the-art IT solutions.",
                Location = "Austin, TX"
            };

            company2.Vacancies = new List<Vacancy>
    {
        new Vacancy
        {
            Id = Guid.Parse("e3b2890f-5a6d-4d6a-bc0f-3f45a7e60f4c"),
            CompanyId = company2.Id,
            Company = company2,
            Title = "Full Stack Developer",
            Description = "InnovateX Systems seeks a Full Stack Developer to design and build robust web applications using modern frameworks like React and Node.js. You will craft scalable solutions that address intricate business needs.",
            CandidateDescription = "Candidates should be fluent in modern web frameworks and demonstrate strong problem-solving abilities.",
            Position = "Cross-Platform Web Creator",
            SalaryMin = 100000f,
            SalaryMax = 140000f,
            WorkMode = VacancyWorkModes.OFFICE,
            LivingConditions = "A dynamic workspace fostering creativity in a collaborative office environment."
        },
        new Vacancy
        {
            Id = Guid.Parse("f7a1b3d9-4e5c-4d8e-b1c3-df2a9e5f6b7c"),
            CompanyId = company2.Id,
            Company = company2,
            Title = "Machine Learning Engineer",
            Description = "InnovateX Systems is on the lookout for a Machine Learning Engineer to develop intelligent algorithms and predictive models. This role combines advanced data analytics with research to drive next-generation innovation.",
            CandidateDescription = "Applicants must demonstrate expertise in TensorFlow, Python, and statistical modeling in real-world projects.",
            Position = "Predictive Systems Designer",
            SalaryMin = 115000f,
            SalaryMax = 155000f,
            WorkMode = VacancyWorkModes.OTHER,
            LivingConditions = "A futuristic workspace with cutting-edge tools and ample collaborative areas."
        },
        new Vacancy
        {
            Id = Guid.Parse("1a2b3c4d-5e6f-4791-a2b3-c4d5e6f789ab"),
            CompanyId = company2.Id,
            Company = company2,
            Title = "Cybersecurity Analyst",
            Description = "InnovateX Systems is hiring a Cybersecurity Analyst to safeguard digital assets by detecting threats and assessing vulnerabilities. You will implement robust security protocols to ensure the integrity of our IT systems.",
            CandidateDescription = "Candidates should have expertise in risk analysis, security frameworks, and incident response management.",
            Position = "Digital Security Consultant",
            SalaryMin = 90000f,
            SalaryMax = 130000f,
            WorkMode = VacancyWorkModes.OFFICE,
            LivingConditions = "A secure and modern environment with a strong focus on advanced cybersecurity measures."
        }
    };

            string password2 = "Innov8#XSys2025";
            data.Add((appUser2, company2, password2));

            // ---------- Company 3: NextGen Solutions ----------
            var appUser3 = new AppUser
            {
                Id = "ac1f4e25-7b8d-4c2e-9d6e-4f3c2b1a0d9e",
                UserName = "NextGenSolutions",
                Email = "info@nextgensolutions.com",
                PhoneNumber = "+1-555-345-6789"
            };

            var company3 = new Company
            {
                Id = Guid.Parse("d0e1f2c3-b4a5-6d7e-8f90-1a2b3c4d5e6f"),
                AppUserId = appUser3.Id,
                AppUser = appUser3,
                SelfDescription = "NextGen Solutions is dedicated to driving business transformation with innovative technology and forward-thinking strategies.",
                Location = "New York, NY"
            };

            company3.Vacancies = new List<Vacancy>
    {
        new Vacancy
        {
            Id = Guid.Parse("b1c2d3e4-f5a6-4789-abcd-1234567890ab"),
            CompanyId = company3.Id,
            Company = company3,
            Title = "Data Scientist",
            Description = "NextGen Solutions seeks a Data Scientist to transform complex datasets into actionable insights. Use big data tools to build predictive models supporting strategic initiatives across the company.",
            CandidateDescription = "Ideal candidates are experts in data mining, statistical analysis, and machine learning with a keen analytical mind.",
            Position = "Insight & Analytics Expert",
            SalaryMin = 105000f,
            SalaryMax = 145000f,
            WorkMode = VacancyWorkModes.OTHER,
            LivingConditions = "A vibrant environment combining dynamic office spaces with flexible work approaches."
        },
        new Vacancy
        {
            Id = Guid.Parse("c2d3e4f5-a6b7-489c-bcde-234567890abc"),
            CompanyId = company3.Id,
            Company = company3,
            Title = "Software Developer",
            Description = "NextGen Solutions is recruiting a Software Developer proficient in modern programming languages. Develop innovative, high-quality software from concept to deployment in a fast-paced, collaborative setting.",
            CandidateDescription = "Candidates need a strong background in coding practices, agile development, and complete lifecycle management.",
            Position = "Code Development Engineer",
            SalaryMin = 95000f,
            SalaryMax = 135000f,
            WorkMode = VacancyWorkModes.OFFICE,
            LivingConditions = "An energetic workplace focused on continuous learning, creativity, and teamwork."
        },
        new Vacancy
        {
            Id = Guid.Parse("d3e4f5a6-b7c8-49ad-bcde-34567890abcd"),
            CompanyId = company3.Id,
            Company = company3,
            Title = "IT Project Manager",
            Description = "NextGen Solutions seeks an IT Project Manager to lead technology initiatives from planning to execution. You will coordinate cross-functional teams, manage budgets, and ensure timely delivery of milestones with advanced project management strategies.",
            CandidateDescription = "Ideal candidates exhibit strong leadership, clear communication, and proven experience in managing complex IT projects.",
            Position = "Technology Initiatives Lead",
            SalaryMin = 110000f,
            SalaryMax = 150000f,
            WorkMode = VacancyWorkModes.OFFICE,
            LivingConditions = "A collaborative environment that emphasizes strategic thinking and innovative project management."
        }
    };

            string password3 = "NextGen!Passw0rd";
            data.Add((appUser3, company3, password3));

            // ---------- Company 4: CyberNova Innovations ----------
            var appUser4 = new AppUser
            {
                Id = "e9f12345-89ab-4cde-8f01-23456789abcd",
                UserName = "CyberNovaInnovations",
                Email = "info@cybernovainnovations.com",
                PhoneNumber = "+1-555-456-7890"
            };

            var company4 = new Company
            {
                Id = Guid.Parse("f0a1b2c3-d4e5-6789-abcd-ef0123456789"),
                AppUserId = appUser4.Id,
                AppUser = appUser4,
                SelfDescription = "CyberNova Innovations pioneers in digital security and cutting-edge software development solutions, delivering innovative strategies for the modern enterprise.",
                Location = "Los Angeles, CA"
            };

            company4.Vacancies = new List<Vacancy>
            {
                new Vacancy
                {
                    Id = Guid.Parse("12345678-90ab-cdef-1234-567890abcdef"),
                    CompanyId = company4.Id,
                    Company = company4,
                    Title = "Security Analyst",
                    Description = "CyberNova Innovations requires a Security Analyst to safeguard our digital assets. This role involves continuous monitoring, risk assessment, and implementing proactive security measures to protect sensitive data and systems.",
                    CandidateDescription = "Ideal candidates possess in-depth cybersecurity knowledge, proficiency in threat detection, and experience with risk management tools.",
                    Position = "Cybersecurity Intelligence Specialist",
                    SalaryMin = 95000f,
                    SalaryMax = 125000f,
                    WorkMode = VacancyWorkModes.OFFICE,
                    LivingConditions = "A collaborative work environment in a modern office space with advanced infrastructure."
                },
                new Vacancy
                {
                    Id = Guid.Parse("abcdef12-3456-7890-abcd-ef1234567890"),
                    CompanyId = company4.Id,
                    Company = company4,
                    Title = "Backend Engineer",
                    Description = "As a Backend Engineer at CyberNova Innovations, you'll develop robust server-side applications and microservices. Your role involves optimizing system performance and integrating APIs for a seamless digital experience.",
                    CandidateDescription = "Candidates should have strong skills in server-side languages, relational databases, and cloud technologies.",
                    Position = "Cloud Services Backend Engineer",
                    SalaryMin = 90000f,
                    SalaryMax = 120000f,
                    WorkMode = VacancyWorkModes.REMOTE,
                    LivingConditions = "Flexible work environment with opportunity for remote work and occasional in-office collaboration."
                },
                new Vacancy
                {
                    Id = Guid.Parse("fedcba98-7654-3210-fedc-ba9876543210"),
                    CompanyId = company4.Id,
                    Company = company4,
                    Title = "Product Manager",
                    Description = "CyberNova Innovations is seeking a Product Manager to drive the strategy and roadmap of our digital products. This role involves market research, stakeholder management, and leading cross-functional teams to deliver innovative solutions.",
                    CandidateDescription = "Experience in agile product management, excellent communication skills, and a strategic mindset are required.",
                    Position = "Digital Products Strategist",
                    SalaryMin = 105000f,
                    SalaryMax = 140000f,
                    WorkMode = VacancyWorkModes.OTHER,
                    LivingConditions = "Dynamic and innovative workspace with opportunities for professional growth and creative collaboration."
                }
            };

            string password4 = "CyberN0va!2025";
            data.Add((appUser4, company4, password4));

            // ---------- Company 5: AICore Dynamics ----------
            var appUser5 = new AppUser
            {
                Id = "a1b2c3d4-e5f6-4789-abcd-abcdef012345",
                UserName = "AICoreDynamics",
                Email = "info@aicoredynamics.com",
                PhoneNumber = "+1-555-567-8901"
            };

            var company5 = new Company
            {
                Id = Guid.Parse("b2c3d4e5-f6a7-4890-bcde-fedcba987654"),
                AppUserId = appUser5.Id,
                AppUser = appUser5,
                SelfDescription = "AICore Dynamics is a leading innovator in artificial intelligence solutions, developing technology to advance cognitive computing.",
                Location = "Boston, MA"
            };

            company5.Vacancies = new List<Vacancy>
            {
                new Vacancy
                {
                    Id = Guid.Parse("c3d4e5f6-a7b8-49c0-bdef-0123456789ab"),
                    CompanyId = company5.Id,
                    Company = company5,
                    Title = "AI Research Engineer",
                    Description = "AICore Dynamics is seeking an AI Research Engineer to develop pioneering algorithms and deep learning models aimed at advancing cognitive computing. Responsibilities include designing neural network architectures and performing data modeling.",
                    CandidateDescription = "Candidates must have strong expertise in machine learning frameworks, deep learning techniques, and statistical analysis.",
                    Position = "Deep Learning Research Engineer",
                    SalaryMin = 120000f,
                    SalaryMax = 170000f,
                    WorkMode = VacancyWorkModes.REMOTE,
                    LivingConditions = "Flexible remote work with state-of-the-art research facilities."
                },
                new Vacancy
                {
                    Id = Guid.Parse("d4e5f6a7-b8c9-40de-abcd-1234567890ac"),
                    CompanyId = company5.Id,
                    Company = company5,
                    Title = "Backend Developer",
                    Description = "Build robust server infrastructures and support scalable API development for AICore Dynamics. The role requires experience with cloud architectures and microservices to empower innovative AI platforms.",
                    CandidateDescription = "Strong proficiency in backend programming languages and cloud technologies is required.",
                    Position = "Cloud-Ready Backend Developer",
                    SalaryMin = 90000f,
                    SalaryMax = 130000f,
                    WorkMode = VacancyWorkModes.OFFICE,
                    LivingConditions = "Modern office located in Boston, MA."
                },
                new Vacancy
                {
                    Id = Guid.Parse("e5f6a7b8-c9d0-41ef-bcde-234567890abc"),
                    CompanyId = company5.Id,
                    Company = company5,
                    Title = "Product Manager",
                    Description = "Oversee the product lifecycle from concept to launch, ensuring product features align with market demands. Collaborate with cross-functional teams to deliver impactful solutions.",
                    CandidateDescription = "Experience in product management and agile methodologies is essential.",
                    Position = "AI Solutions Product Owner",
                    SalaryMin = 100000f,
                    SalaryMax = 140000f,
                    WorkMode = VacancyWorkModes.OTHER,
                    LivingConditions = "Collaborative environment with flexible work arrangements."
                }
            };

            string password5 = "AICore@2025Lab";
            data.Add((appUser5, company5, password5));


            // ---------- Company 6: Quantum AI Labs ----------
            var appUser6 = new AppUser
            {
                Id = "a3b4c5d6-e7f8-4901-ace2-bcdef12345678",
                UserName = "QuantumAILabs",
                Email = "info@quantumailabs.com",
                PhoneNumber = "+1-555-789-0123"
            };

            var company6 = new Company
            {
                Id = Guid.Parse("d4e5f6a7-b8c9-4d01-ab12-3c4d5e6f7890"),
                AppUserId = appUser6.Id,
                AppUser = appUser6,
                SelfDescription = "Quantum AI Labs is at the forefront of innovation, merging quantum computing with artificial intelligence to deliver next-generation digital solutions.",
                Location = "San Jose, CA",
            };

            company6.Vacancies = new List<Vacancy>
            {
                new Vacancy
                {
                    Id = Guid.Parse("b2c3d4e5-f6a7-4b89-cdef-0123456789ac"),
                    CompanyId = company6.Id,
                    Company = company6,
                    Title = "Data Engineer",
                    Description = "Quantum AI Labs requires a Data Engineer to build and manage scalable data pipelines, ensuring data integrity and real-time processing for advanced analytics.",
                    CandidateDescription = "Expertise in SQL, NoSQL, and big data technologies is essential.",
                    Position = "Big Data Systems Engineer",
                    SalaryMin = 100000f,
                    SalaryMax = 135000f,
                    WorkMode = VacancyWorkModes.OFFICE,
                    LivingConditions = "Modern office with robust technical infrastructure.",
                },
                new Vacancy
                {
                    Id = Guid.Parse("c3d4e5f6-a7b8-4c90-def0-1234567890bd"),
                    CompanyId = company6.Id,
                    Company = company6,
                    Title = "AI Product Manager",
                    Description = "Oversee the lifecycle of AI-powered products, bridging technical development with market strategy. Drive innovation and align product features with customer needs in a competitive landscape.",
                    CandidateDescription = "Candidates must have strong experience in product management and a solid understanding of AI technologies.",
                    Position = "AI-Driven Product Lead",
                    SalaryMin = 115000f,
                    SalaryMax = 155000f,
                    WorkMode = VacancyWorkModes.REMOTE,
                    LivingConditions = "Flexible remote work with frequent collaborative strategy sessions.",
                },
                new Vacancy
                {
                    Id = Guid.Parse("d3e4f5a6-a7b8-4d91-ef01-23456789abef"),
                    CompanyId = company6.Id,
                    Company = company6,
                    Title = "Quality Assurance Engineer",
                    Description = "Ensure the reliability and performance of Quantum AI Labs' software products through rigorous testing and quality control processes. Identify defects and drive continuous improvement.",
                    CandidateDescription = "A keen eye for detail and proficiency in testing methodologies are required.",
                    Position = "AI Software Quality Specialist",
                    SalaryMin = 85000f,
                    SalaryMax = 115000f,
                    WorkMode = VacancyWorkModes.OTHER,
                    LivingConditions = "A supportive work environment that fosters continuous innovation.",
                }
            };

            string password6 = "QuantumAI#2025Lab";
            data.Add((appUser6, company6, password6));

            return data;
        }
    }
}