using Microsoft.AspNetCore.Mvc;
using BusinessRuleEngine.Entities; // import the Rule class from the entities folder
using BusinessRuleEngine.DTO;
using Rule = BusinessRuleEngine.Entities.Rule;
using BusinessRuleEngine.Repositories; // import the repositories folder from the project
using System.Diagnostics;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BusinessRuleEngine.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AddRuleController : ControllerBase
    {
        // used to import the sql repository to read all the rules from
        private readonly SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;

        public AddRuleController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration, "RuleTable"); // pass in data retrieved from server to instance of SQLRepository
        }

        // returns a json formatted result of the rules saved on an sql database specified under "appsettings.json"
        // GET: /<GetAllRules>
        [HttpGet]
        [Route("GetAllRules")] // change this to change name on swaggerUI
        public IEnumerable<Rule> Get()
        {
            // create an instance of Response to return any possible errors
            Response response = new Response();

            // get the rules from the sql repository and save as a variable
            var rulesList = sqlRepo.getAllRules();

            // if the arraylist has at least 1 item, then return it as a json object
            /*if (rulesList.Count > 0)
            {
                Console.WriteLine(JsonConvert.SerializeObject(rulesList).GetType());
                return rulesList;
            }*/

            // Debug.WriteLine("size of the list: " +info.Count);
            return rulesList;
            
        }


        // PUT /AddRule
        // add a rule based on what the user has sent (ruleID and expressionID are generated randomly)
        [HttpPut]
        [Route("AddRule")]
        public void CreateRule(CreateRuleDTO ruleDTO)
        {
            if (sqlRepo.ruleExists(ruleDTO.RuleName))
            {
                Debug.WriteLine("The rule named " + ruleDTO.RuleName + " already exists");
                return;
            }

            if (!sqlRepo.IsValidExpressionId(ruleDTO.ExpressionID))
            {
                Debug.WriteLine("Invalid expression ID: " + ruleDTO.ExpressionID);
                return;
            }

            if (sqlRepo.RuleExistsByValue(
                sqlRepo.getAllRules(),
                ruleDTO.PositiveAction,
                ruleDTO.PositiveValue,
                ruleDTO.NegativeAction,
                ruleDTO.NegativeValue))
            {
                Debug.WriteLine("A Rule with the same properties already exists.");
                return;
            }

            // get all the elements needed to create a rule
            Rule newRule = new Rule
            {
                RuleID = Guid.NewGuid().ToString(),
                RuleName = ruleDTO.RuleName,
                ExpressionID = ruleDTO.ExpressionID,
                PositiveAction = ruleDTO.PositiveAction,
                PositiveValue = ruleDTO.PositiveValue,
                NegativeAction = ruleDTO.NegativeAction,
                NegativeValue = ruleDTO.NegativeValue
            };

            sqlRepo.addRule(newRule);

            Debug.WriteLine("The values in body: " + newRule);
            //return CreatedAtAction()
        }


        [HttpPut]
        [Route("EditRule")]
        public void EditRule(EditRuleDTO ruleDTO)
        {
            // Get the ID of the rule that is going to be edited
            string ruleIDofRuleToEdit = ruleDTO.RuleID;
            
            // Check if the rule exists in the database
            if (!sqlRepo.ruleIDExists(ruleIDofRuleToEdit))
            {
                Debug.WriteLine("The rule with ID " + ruleIDofRuleToEdit + " does not exist");
                return;
            }
            
            
            // Check if the expression ID is valid
            if (!sqlRepo.IsValidExpressionId(ruleDTO.ExpressionID))
            {
                Debug.WriteLine("Invalid expression ID: " + ruleDTO.ExpressionID);
                return;
            }
            
            // Edit the rule
            sqlRepo.editRule(ruleDTO);
            Debug.WriteLine("The values in body: " + ruleIDofRuleToEdit);
        }


        // TODO: Add functionallity to remove rule from database
        [HttpDelete]
        [Route("DeleteRule")]
        public void DeleteRule(string ruleID, string expressionID)
        {
            // check that the rule exists
            if (!sqlRepo.ruleIDExists(ruleID))
            {
                Debug.WriteLine("The rule with ID " + ruleID + " does not exist");
                return;
            }

            // check that the expression ID is valid
            if (!sqlRepo.expressionExists(expressionID))
            {
                Debug.WriteLine("Invalid expression ID: " + expressionID);
                return;
            }

            // delete the rule
            sqlRepo.deleteRule(ruleID);

            Debug.WriteLine("Rule with ID " + ruleID + " was deleted");
        }


    }
}
