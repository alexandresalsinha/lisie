package com.example.spirostockmanagementandroid;

import java.io.File;
import java.io.IOException;
import java.sql.Date;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;
import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathExpression;
import javax.xml.xpath.XPathExpressionException;
import javax.xml.xpath.XPathFactory;

//import me.tests.R;

import org.w3c.dom.DOMException;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.w3c.dom.UserDataHandler;
import org.xml.sax.SAXException;

import android.text.format.DateFormat;
import android.widget.EditText;

public class GlobalFunctions {
	//public String InventoryItemsXmlPath = "/storage/extSdCard/SpiroSMDatabases/InventoryItems.xml";
	public String InventoryItemsXmlPath = "/Primary/SpiroStockManagement/InventoryItems.xml";
	public Document GetDocument() {
		DocumentBuilderFactory domFactory = DocumentBuilderFactory
				.newInstance();
		domFactory.setNamespaceAware(true); // never forget this!
		DocumentBuilder builder;
		Document doc = null;
		//String youFilePath = Environment.getExternalStorageDirectory()
			//	.toString() + "/SpiroSMDatabases/Recepies.xml";

		try {
			builder = domFactory.newDocumentBuilder();

			doc = builder.parse(new File(InventoryItemsXmlPath));
		} catch (ParserConfigurationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (SAXException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return doc;
	}

	public NodeList GetNodesOfXpath(String xpathstring) {
		DocumentBuilderFactory domFactory = DocumentBuilderFactory
				.newInstance();
		domFactory.setNamespaceAware(true); // never forget this!
		DocumentBuilder builder;
		Document doc = null;
		//String youFilePath = Environment.getExternalStorageDirectory()
		//		.toString() + "/SpiroSMDatabases/Recepies.xml";
		
		try {
			builder = domFactory.newDocumentBuilder();

			doc = builder.parse(new File(InventoryItemsXmlPath));
		} catch (ParserConfigurationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (SAXException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		XPathFactory factory = XPathFactory.newInstance();
		XPath xpath = factory.newXPath();
		XPathExpression expr;
		try {
			// expr = xpath.compile("//Recipe[Id='" + value + "']/Name");
			expr = xpath.compile(xpathstring);

			// NodeList nodess = doc.getElementsByTagName("Recipe");
			Object result = expr.evaluate(doc, XPathConstants.NODESET);
			return (NodeList) result;
		} catch (XPathExpressionException _e) {
			// TODO Auto-generated catch block
			_e.printStackTrace();
		}
		return null;
	}

	public NodeList GetNodesOfXpath(Document xmlDocument, String xpathstring) {
		XPathFactory factory = XPathFactory.newInstance();
		XPath xpath = factory.newXPath();
		XPathExpression expr;
		try {
			// expr = xpath.compile("//Recipe[Id='" + value + "']/Name");
			expr = xpath.compile(xpathstring);

			// NodeList nodess = doc.getElementsByTagName("Recipe");
			Object result = expr.evaluate(xmlDocument, XPathConstants.NODESET);
			return (NodeList) result;
		} catch (XPathExpressionException _e) {
			// TODO Auto-generated catch block
			_e.printStackTrace();
		}
		return null;
	}

	public ArrayList<String> GetCategoriesList() {
		NodeList _nodes = GetNodesOfXpath("//Product/categoryString");

		boolean _found = false;
		ArrayList<String> _categories = new ArrayList<String>();
		if (_nodes != null && _nodes.getLength() > 0) {

			_categories.add("Todas");
			for (int z = 0; z < _nodes.getLength(); z++) {
				Element _element = (Element) _nodes.item(z);
				String _categoryName = _element.getTextContent();

				if (_categories.size() == 0) {
					_categories.add(_categoryName);
					continue;
				}

				_found = false;

				for (String _s : _categories) {
					//if (_s.equals(_categoryName) ||_categoryName.isEmpty()) {
					if (_s.equals(_categoryName) || ("").equals(_categoryName)) {
						_found = true;
						break;
					}
				}
				if (!_found)
					_categories.add(_categoryName);
			}
		}
		return _categories;
	}

	public Boolean SaveXmlDocument(Document _doc) {

		TransformerFactory transformerFactory = TransformerFactory
				.newInstance();
		Transformer transformer;
		try {
			transformer = transformerFactory.newTransformer();
			transformer.setOutputProperty(OutputKeys.INDENT, "yes");
			transformer.setOutputProperty(
					"{http://xml.apache.org/xslt}indent-amount", "2");
			
			//String youFilePath = Environment.getExternalStorageDirectory()
			//		.toString() + "/SpiroSMDatabases/Recepies.xml";
			DOMSource source = new DOMSource(_doc);
			StreamResult streamResult = new StreamResult(new File(InventoryItemsXmlPath));
			try {
				transformer.transform(source, streamResult);
			} catch (TransformerException e) {

				e.printStackTrace();
				return false;
			}
		} catch (TransformerConfigurationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return true;

	}

	
	
	
	public void AddProductToList(Document doc, int productId, String list, String quantity)
	{
		//Document _doc = GetDocument();
		if(doc == null)
			doc = GetDocument();
		
		NodeList _nodes = GetNodesOfXpath(doc, "//Product[Id='" + productId + "']");

		//TODO quantity in weight
		if (_nodes != null && _nodes.getLength() > 0) {
			Element _element = (Element) _nodes.item(0);
			if(("in").equals(list))
			{
				
				Element _quantityIn = (Element)_element.getElementsByTagName("QuantityIn").item(0);
				Integer _currentQuantityIn = Integer.parseInt(XmlFunctions.getValue(_element, "QuantityIn"));
				Integer _quantityToAdd = Integer.parseInt(quantity);
				Integer _newQuantityIn = _currentQuantityIn.intValue() + _quantityToAdd.intValue();
				_quantityIn.setTextContent(String.valueOf(_newQuantityIn));
			}
			//out
			else 
			{
				Element _quantityOut = (Element)_element.getElementsByTagName("QuantityOut").item(0);
				Integer _currentQuantityOut = Integer.parseInt(XmlFunctions.getValue(_element, "QuantityOut"));
				Integer _quantityToAdd = Integer.parseInt(quantity);
				Integer _newQuantityOut = _currentQuantityOut.intValue() + _quantityToAdd.intValue();
				_quantityOut.setTextContent(String.valueOf(_newQuantityOut));
			}
			
			
			Element _itemEL = doc.createElement("Item");
			
			Element _quantityEL = doc.createElement("Quantity");
			_quantityEL.setTextContent(quantity);
			
			Element _QuantityWeightEL = doc.createElement("QuantityWeight");
			_QuantityWeightEL.setTextContent("0");
			
			Element _ListNameEL = doc.createElement("ListName");
			_ListNameEL.setTextContent(list);
			
			Element _InsertDateEL = doc.createElement("InsertDate");
			_InsertDateEL.setTextContent(GetCurrentTimeLong());
			
//			<Quantity>6</Quantity>
//	        <QuantityWeight>0</QuantityWeight>
//	        <ListName>out</ListName>
//	        <InsertDate>2011-11-28T13:15:37</InsertDate>
			_itemEL.appendChild(_quantityEL);
			_itemEL.appendChild(_QuantityWeightEL);
			_itemEL.appendChild(_ListNameEL);
			_itemEL.appendChild(_InsertDateEL);
			
			//check if history exists
			//if it does append an Item to it
			NodeList _historyNL = _element.getElementsByTagName("History");
			if (_historyNL.getLength() > 0)
			{
				Element _historyEL = (Element)_historyNL.item(0);
				_historyEL.appendChild(_itemEL);
			}
			//Create History Node because is inexistente
			else {
				Element _newHistoryEl = doc.createElement("History");
				_newHistoryEl.appendChild(_itemEL);
				_element.appendChild(_newHistoryEl);
			}
		
			SaveXmlDocument(doc);
		}
	}

	
	public void AddProductNew(String name, String category, String brand, String price, String priceWeight, String packageInfo, String barCode, String list, String quantity)
	{
		Document _doc = GetDocument();
		
		//Create the xml Elements
		Element _rootElement = _doc.createElement("Product");
		 
		 Element _ElementId = _doc.createElement("Id");
		 Element _ElementName = _doc.createElement("Name");
		 Element _ElementPrice = _doc.createElement("Price");
		 Element _ElementVariableWeightPrice = _doc.createElement("VariableWeightPrice");
		 Element _ElementBrand = _doc.createElement("Brand");
		 Element _ElementPackageInfo = _doc.createElement("PackageInfo");
		 Element _ElementcategoryString = _doc.createElement("categoryString");
		 Element _ElementPictureSmallFilename = _doc.createElement("PictureSmallFilename");
		 Element _ElementInformationTakenFrom = _doc.createElement("InformationTakenFrom");
		 Element _ElementMarketItemUrl = _doc.createElement("MarketItemUrl");
		 Element _ElementBarCode = _doc.createElement("BarCode");
		 Element _ElementIsBlackListed = _doc.createElement("IsBlackListed");
		 Element _ElementInsertDate = _doc.createElement("InsertDate");
		 Element _ElementQuantityIn = _doc.createElement("QuantityIn");
		 Element _ElementQuantityWeightIn = _doc.createElement("QuantityWeightIn");
		 Element _ElementQuantityOut = _doc.createElement("QuantityOut");
		 Element _ElementQuantityWeightOut = _doc.createElement("QuantityWeightOut");
		 Element _ElementHistory = _doc.createElement("History");
		 
		 int _newId = GetLasProductId(_doc) + 1;
		 
		 //Set Xml Elements Values
		 _ElementId.appendChild(_doc.createTextNode(Integer.toString(_newId)));
		 _ElementName.appendChild(_doc.createTextNode(name));
		 _ElementPrice.appendChild(_doc.createTextNode(price));
		 _ElementVariableWeightPrice.appendChild(_doc.createTextNode(priceWeight));
		 _ElementBrand.appendChild(_doc.createTextNode(brand));
		 _ElementPackageInfo.appendChild(_doc.createTextNode(packageInfo));
		 _ElementcategoryString.appendChild(_doc.createTextNode(category));
		 _ElementPictureSmallFilename.appendChild(_doc.createTextNode(Integer.toString(_newId) + "small.jpg"));
		 _ElementInformationTakenFrom.appendChild(_doc.createTextNode(""));
		 _ElementMarketItemUrl.appendChild(_doc.createTextNode(""));
		 _ElementBarCode.appendChild(_doc.createTextNode(barCode));
		 _ElementIsBlackListed.appendChild(_doc.createTextNode("false"));
		 _ElementInsertDate.appendChild(_doc.createTextNode(GetCurrentTimeLong()));
		 _ElementQuantityIn.appendChild(_doc.createTextNode("0"));
		 _ElementQuantityWeightIn.appendChild(_doc.createTextNode("0"));
		 _ElementQuantityOut.appendChild(_doc.createTextNode("0"));
		 _ElementQuantityWeightOut.appendChild(_doc.createTextNode("0"));
		 //_ElementHistory.appendChild(_doc.createTextNode());
		 
		 
		 //Append the Xml Elements to the main Xml Element
		 _rootElement.appendChild(_ElementId);
		 _rootElement.appendChild(_ElementName);
		 _rootElement.appendChild(_ElementPrice);
		 _rootElement.appendChild(_ElementVariableWeightPrice);
		 _rootElement.appendChild(_ElementBrand);
		 _rootElement.appendChild(_ElementPackageInfo);
		 _rootElement.appendChild(_ElementcategoryString);
		 _rootElement.appendChild(_ElementPictureSmallFilename);
		 _rootElement.appendChild(_ElementInformationTakenFrom);
		 _rootElement.appendChild(_ElementMarketItemUrl);
		 _rootElement.appendChild(_ElementBarCode);
		 _rootElement.appendChild(_ElementIsBlackListed);
		 _rootElement.appendChild(_ElementInsertDate);
		 _rootElement.appendChild(_ElementQuantityIn);
		 _rootElement.appendChild(_ElementQuantityWeightIn);
		 _rootElement.appendChild(_ElementQuantityOut);
		 _rootElement.appendChild(_ElementQuantityWeightOut);
		 _rootElement.appendChild(_ElementHistory);
		 
		 _doc.getFirstChild().appendChild(_rootElement);
		 
		 if(list == "in" || list == "out")
			 AddProductToList(_doc, _newId, list, quantity);
		 
		 SaveXmlDocument(_doc);
	}
	
	
	public int GetLasProductId(Document doc)
	{
		if(doc == null)
			doc = GetDocument();
		
		NodeList _nodesLastId = GetNodesOfXpath(doc, "/InventoryList/Product[last()]");
		 Element _elementLastNode = (Element)_nodesLastId.item(0); 
		 int _lastId =Integer.parseInt(_elementLastNode.getElementsByTagName("Id").item(0).getTextContent());
		
		return _lastId;
	}
	
	public Boolean DeleteRecipe(String recipeId) {
		Document _doc = GetDocument();

		NodeList _nodes = GetNodesOfXpath(_doc, "//Recipe[Id='" + recipeId + "']");

		if (_nodes != null && _nodes.getLength() > 0) {
			Element _element = (Element) _nodes.item(0);
			_element.getParentNode().removeChild(_element);
		}

		SaveXmlDocument(_doc);
		return true;
	}
	
	public Boolean DeleteIngredientFromRecipe(String recipeId,
			String ingredientName) {
		Document _doc = GetDocument();

		NodeList _nodes = GetNodesOfXpath(_doc, "//Recipe[Id='" + recipeId
				+ "']/IngredientList/RecipeIngredient[Name='" + ingredientName
				+ "']");

		if (_nodes != null && _nodes.getLength() > 0) {
			Element _element = (Element) _nodes.item(0);
			_element.getParentNode().removeChild(_element);
		}

		SaveXmlDocument(_doc);
		return true;
	}
	public Boolean DeleteDirectionStepFromRecipe(String recipeId,
			String stepValue) {
		Document _doc = GetDocument();

		NodeList _nodes = GetNodesOfXpath(_doc, "//Recipe[Id='" + recipeId
				+ "']/Directions/Step[Value='" + stepValue
				+ "']");

		if (_nodes != null && _nodes.getLength() > 0) {
			Element _element = (Element) _nodes.item(0);
			_element.getParentNode().removeChild(_element);
		}

		SaveXmlDocument(_doc);
		return true;
	}
	
	public String GetCurrentTimeLong()
	{
		Calendar _c = Calendar.getInstance();
		SimpleDateFormat _sdf = new SimpleDateFormat("yyyy-MM-dd");
		SimpleDateFormat _sdf2 = new SimpleDateFormat("hh:mm:ss");
		return _sdf.format(_c.getTime()) + "T" + _sdf2.format(_c.getTime());
	}
	
	public NodeList GetProductsOfListAndCategory(String list, String category)
	{
		if(("in").equals(list))
		{
			if(("").equals(category))
				return GetNodesOfXpath("//Product[QuantityIn > 0]");
			
			return GetNodesOfXpath("//Product[categoryString='" + category + "' and QuantityIn > 0]");
		}
		if(("out").equals(list))
		{
			if(("").equals(category))
				return GetNodesOfXpath("//Product[QuantityOut > 0]");
			
			return GetNodesOfXpath("//Product[categoryString='" + category + "' and QuantityOut > 0]");
		}
		return null;
	}

	public void RemoveQuantityFromInventoryOfProduct(int productId, String quantity)
	{
		Document doc = GetDocument();
		NodeList _nodes = GetNodesOfXpath(doc, "//Product[Id='" + productId + "']");

		//TODO quantity in weight
		if (_nodes != null && _nodes.getLength() > 0) 
		{
			Element _element = (Element) _nodes.item(0);
			if(_element != null)
			{
				Element _quantityIn = (Element)_element.getElementsByTagName("QuantityIn").item(0);
				Integer _newQuantityIn = 0;
				Integer _currentQuantityIn = Integer.parseInt(XmlFunctions.getValue(_element, "QuantityIn"));
				if(_currentQuantityIn - Integer.parseInt(quantity) > 0)
					_newQuantityIn = _currentQuantityIn - Integer.parseInt(quantity);
				
				_quantityIn.setTextContent(String.valueOf(_newQuantityIn));
				
				SaveXmlDocument(doc);
			}
		}
	}
	
	public void RemoveQuantityFromCartOfProduc(int productId, String quantity)
	{
		Document doc = GetDocument();
		NodeList _nodes = GetNodesOfXpath(doc, "//Product[Id='" + productId + "']");

		//TODO quantity in weight
		if (_nodes != null && _nodes.getLength() > 0) 
		{
			Element _element = (Element) _nodes.item(0);
			if(_element != null)
			{
				Element _quantityOut = (Element)_element.getElementsByTagName("QuantityOut").item(0);
				Integer _newQuantityOut = 0;
				Integer _currentQuantityOut = Integer.parseInt(XmlFunctions.getValue(_element, "QuantityOut"));
				if(_currentQuantityOut - Integer.parseInt(quantity) > 0)
					_newQuantityOut = _currentQuantityOut - Integer.parseInt(quantity);
				
				_quantityOut.setTextContent(String.valueOf(_newQuantityOut));
				
				SaveXmlDocument(doc);
			}
		}
	}
}
