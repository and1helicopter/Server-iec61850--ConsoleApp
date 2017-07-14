using System;
using IEC61850.Client;
using IEC61850.Common;
using IEC61850.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void CreateModelFromNonExistingFile()
		{
			IedModel iedModel = ConfigFileParser.CreateModelFromConfigFile("test.cfg");

			Assert.IsNotNull(iedModel);
		}

		[TestMethod]
		public void AccessDataModelServerSide()
		{
			IedModel iedModel = ConfigFileParser.CreateModelFromConfigFile("test.cfg");

			ModelNode modelNode = iedModel.GetModelNodeByShortObjectReference("LD0/4AnIn_GGIO2.AnIn1");

			Assert.IsTrue(modelNode.GetType().Equals(typeof(DataObject)));

			modelNode = iedModel.GetModelNodeByShortObjectReference("LD0/4AnIn_GGIO2.AnIn1.mag.f");

			Assert.IsTrue(modelNode.GetType().Equals(typeof(IEC61850.Server.DataAttribute)));

			Assert.IsNotNull(modelNode);
		}


		public delegate MmsDataAccessError DCallback(IEC61850.Server.DataAttribute dataAttr, MmsValue value, ClientConnection con,object parameter);
		//{
		//	return MmsDataAccessError.SUCCESS;
		//};


		//hendler 
		MmsDataAccessError lol(IEC61850.Server.DataAttribute dataAttr, MmsValue value, ClientConnection con,
			object parameter)
		{
			return MmsDataAccessError.SUCCESS;
		}


		[TestMethod]
		public void WriteAccessPolicy()
		{


			IedModel iedModel = ConfigFileParser.CreateModelFromConfigFile("test.cfg");

			IEC61850.Server.DataAttribute opDlTmms = (IEC61850.Server.DataAttribute)iedModel.GetModelNodeByShortObjectReference("LD0/4AnIn_GGIO2.AnIn1.mag.f");

			ModelNode modelNode = iedModel.GetModelNodeByShortObjectReference("LD0/4AnIn_GGIO2.AnIn1.mag.f");

			Assert.IsTrue(modelNode.GetType().Equals(typeof(IEC61850.Server.DataAttribute)));

			IEC61850.Server.DataAttribute rsDlTmms = (IEC61850.Server.DataAttribute)iedModel.GetModelNodeByShortObjectReference("LD0/4AnIn_GGIO2.AnIn2.mag.f");

			IedServer iedServer = new IedServer(iedModel);



			iedServer.HandleWriteAccess(opDlTmms, lol, null);


			iedServer.Start(10002);

			IedConnection connection = new IedConnection();

			connection.Connect("localhost", 10002);

			connection.WriteValue("LD0/4AnIn_GGIO2.AnIn2.mag.f", FunctionalConstraint.SP, new MmsValue(100));

			iedServer.SetWriteAccessPolicy(FunctionalConstraint.SP, AccessPolicy.ACCESS_POLICY_DENY);

			connection.WriteValue("LD0/4AnIn_GGIO2.AnIn1.mag.f", FunctionalConstraint.SP, new MmsValue(100));

			try
			{
				connection.WriteValue("LD0/4AnIn_GGIO2.AnIn1.mag.f", FunctionalConstraint.SP, new MmsValue(13));
			}
			catch (IedConnectionException e)
			{
				Assert.AreEqual(IedClientError.IED_ERROR_ACCESS_DENIED, e.GetIedClientError());
			}

			MmsValue rsDlTmmsValue = iedServer.GetAttributeValue(rsDlTmms);

			Assert.AreEqual(100, rsDlTmmsValue.ToInt32());

			connection.Abort();

			iedServer.Stop();

			iedServer.Destroy();
		}
	}
}
