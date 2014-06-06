
public enum DataBaseTypeEnums
{
   SqlServerByTextQuery = 1,
   SqlServerByCommandQuery = 2,
   MongoDbNormalizedWriteAck = 3,
   MongoDbNormalizedWriteUnack = 4,
   MongoDbDenormalizedWriteAck = 5,
   MongoDbDenormalizedWriteUnack = 6,
}

public enum TestCaseEnums
{
   UnitTest = -1,
   TestCase1 = 1,
   TestCase2 = 2,
   TestCase3 = 3,
   TestCase4 = 4,
   TestCase5 = 5,
   TestCase6 = 6,
   TestCase7 = 7,
   TestCase8 = 8,
}

public enum TestScenarioEnums
{
   InsertDepartment = 1,
   InsertProject = 2,
   InsertUser = 3,
   UpdateDepartmentNameByKeys = 4,
   UpdateProjectNameByKeys = 5,
   UpdateUserLastNameByFirstName = 6,
   SelectDepartmentByKey = 7,
   SelectDepartmentByRandomName = 8,
   SelectUserByKey = 9,
   SelectUsersByRandomFirstName = 10,
   SelectDepartmentByRandomUserFirstName = 11,
   SelectUsersByRandomProjectKeys = 12,
   SelectAverageAgeByRandomProjectKeys = 13,
}