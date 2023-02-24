using Microsoft.AspNetCore.Mvc;
using BusinessRuleEngine.DTO;
using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Repositories;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BusinessRuleEngine.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AddExpressionController : ControllerBase
    {
        // used to import the sql repository to read all the rules from
        private readonly SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;

        public AddExpressionController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration, "ExpressionTable"); // pass in data retrieved from server to instance of SQLRepository
        }

        // returns a json formatted result of the rules saved on an sql database specified under "appsettings.json"

        // GET: /GetAllExpressions
        [HttpGet]
        [Route("GetAllExpressions")]
        public IEnumerable<Expression> Get()
        {
            // create an instance of Response to return any possible errors
            Response response = new Response();

            // get the rules from the sql repository and save as a variable
            var expressionsList = sqlRepo.getAllExpressions();

            // Debug.WriteLine("size of the list: " +info.Count);
            return expressionsList;
        }


        // PUT /AddExpression
        [HttpPut]
        [Route("AddExpression")]
        public void createExpression(CreateExpressionDTO expressionDTO)
        {
            // TODO: Verify that the expression doesn't already exist
            List<Expression> existingExpressions = sqlRepo.getAllExpressions();
            bool expressionExists = sqlRepo.ExpressionExistsByValue(
                                    existingExpressions,
                                expressionDTO.LeftOperandType,
                                expressionDTO.LeftOperandValue,
                                expressionDTO.RightOperandType,
                                expressionDTO.RightOperandValue,
                                    expressionDTO.Operator);
            
            if (expressionExists)
                {
                    // Return an error response indicating that the expression already exists
                    Debug.WriteLine("An expression with the same properties already exists.");
            }else{
            // get all the elements needed to create an Expression
            Expression newExpression = new Expression
            {
                ExpressionID = Guid.NewGuid().ToString(),
                LeftOperandType = expressionDTO.LeftOperandType,
                LeftOperandValue = expressionDTO.LeftOperandValue,
                RightOperandType = expressionDTO.RightOperandType,
                RightOperandValue = expressionDTO.RightOperandValue,
                Operator = expressionDTO.Operator
            };
            

            sqlRepo.addExpression(newExpression);

            Debug.WriteLine("The values in body: " + newExpression);
            //return CreatedAtAction()
            }
        }

        [HttpPut]
        [Route("EditExpression")]
        public void EditRule(EditExpressionDTO expressionDTO)
        {
            // get the name of the rule that is going to be added
            string expressionIDofExpressionToEdit = expressionDTO.ExpressionID;

            //TODO: Needs to check if rule doesn't exist and handle accordingly
            if (sqlRepo.expressionExists(expressionIDofExpressionToEdit))
            {
                Debug.WriteLine("The expression id " + expressionIDofExpressionToEdit + " already exists");
            }

            // TODO: if the rule does not exist, make sure that the expressionID is valid
            sqlRepo.editExpression(expressionDTO);

            Debug.WriteLine("The values in body: " + expressionIDofExpressionToEdit);
            //return CreatedAtAction()
        }

        [HttpDelete]
        [Route("DeleteExpression")]
        public void deleteExpression(DeleteExpressionDTO expressionDTO)
        {
            // TODO: Verify that the expression doesn't already exist

            // get all the elements needed to create an Expression

            string expressionToDeleteID = expressionDTO.expressionID;

            if (!sqlRepo.ruleExists(expressionToDeleteID))
            {
                Debug.WriteLine("The rule named " + expressionToDeleteID + " doesn't exists");
            }

            sqlRepo.deleteExpression(expressionToDeleteID);



            Debug.WriteLine("The values in body: " + expressionToDeleteID);
            //return CreatedAtAction()
        }

        // [HttpGet]
        // [Route("/GetLeftOperandType")]
        //  public ActionResult<IEnumerable<Expression>> GetExpressionsByLeftOperandType(string LeftOperandType) //GetExpression DTO is redundant
        // {
        //     var expressions = sqlRepo.GetExpressionsByLeftOperandType(LeftOperandType); 
        //     if( expressions == null)
        //     {
        //         return NotFound();
        //     }
        //     return Ok(expressions);
        // }
    }
}
