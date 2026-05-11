package com.example.spirostockmanagementandroid;

import java.io.File;
import java.io.FileInputStream;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathExpression;
import javax.xml.xpath.XPathExpressionException;
import javax.xml.xpath.XPathFactory;

//import me.tests.GlobalFunctions;
//import me.tests.Helloworld2Activity;
//import me.tests.R;
//import me.tests.ViewRecepyActivity;
//import me.tests.XMLfunctions;

//import me.tests.GlobalFunctions;
//import me.tests.Helloworld2Activity;
//import me.tests.R;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

import android.R.bool;
import android.os.Bundle;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.view.ContextMenu;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ContextMenu.ContextMenuInfo;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

public class ViewProductsList extends Activity implements OnItemSelectedListener {

	boolean _isStarting = true; 
	String ListToView = "out";
	
	
	List<HashMap<String, String>> ProductsOfListListMap = new ArrayList<HashMap<String, String>>();
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_view_products_list);
        
        Intent intent = getIntent();
        ListToView = intent.getStringExtra("listToOpen");
        
        
        ListView lv = (ListView) findViewById(R.id.listView_ProductsOfList);
		lv.setOnLongClickListener(new View.OnLongClickListener() {

			public boolean onLongClick(View v) {
				// TODO Auto-generated method stub
				// v.setBackgroundColor(Color.GRAY);
				v.showContextMenu();
				return true;
			}
		});
		
		Spinner spinner = (Spinner) findViewById(R.id.spinner_ProductAddToList_Options);
		
        InitializeProductCategories();
        InitializeProductsOfCategory("Todas");
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.activity_view_products_list, menu);
        return true;
    }
    
    public void InitializeProductCategories() {
		GlobalFunctions _gf = new GlobalFunctions();
		ArrayList<String> _categories = _gf.GetCategoriesList();

		Spinner _SpinnerCategories = (Spinner) findViewById(R.id.spinner_List_ProductCategories);
		_SpinnerCategories.setAdapter(null);

		ArrayAdapter<String> adapter = new ArrayAdapter<String>(
				ViewProductsList.this, android.R.layout.simple_spinner_item,
				_categories);
		adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
		_SpinnerCategories.setAdapter(adapter);
		_SpinnerCategories.setOnItemSelectedListener(this);
		adapter.notifyDataSetChanged();

	}
    
  //Category DropDown Item Selected
    public void onItemSelected(AdapterView<?> parent, View view, int pos, long id) {
  		// TODO Auto-generated method stub
  		if (_isStarting) {
  			_isStarting = false;
  			return;
  		}
  		Spinner _SpinnerCategories = (Spinner) findViewById(R.id.spinner_List_ProductCategories);
  		String Text = _SpinnerCategories.getSelectedItem().toString();
  		InitializeProductsOfCategory(Text);

  	}

  	public void onNothingSelected(AdapterView<?> arg0) {
  		// TODO Auto-generated method stub

  	}
  	
    public void InitializeProductsOfCategory(String category) {

    	GlobalFunctions _gb = new GlobalFunctions();
		NodeList _nodesToPopulate = null;

		ListView lv = (ListView) findViewById(R.id.listView_ProductsOfList);
		lv.setAdapter(null);

		if (category.equals("Todas")) 
		{
			_nodesToPopulate = _gb.GetProductsOfListAndCategory(ListToView, "");
			
		} 
		else 
		{
			_nodesToPopulate = _gb.GetProductsOfListAndCategory(ListToView, category);
		}

		// create the grid item mapping

		String[] from = new String[] { "rowName", "rowId" };

		int[] to = new int[] { R.id.textView_ProductList_GridItem_Text, R.id.textView_ProductList_GridItem_id };

		// prepare the list of all records

		List<HashMap<String, String>> fillMaps = new ArrayList<HashMap<String, String>>();

		for (int i = 0; i < _nodesToPopulate.getLength(); i++) {
			Element e = (Element) _nodesToPopulate.item(i);
			HashMap<String, String> map = new HashMap<String, String>();

			String _rowText = XmlFunctions.getValue(e, "Name");
			if(("in").equals(ListToView))
					_rowText = _rowText + "(" + XmlFunctions.getValue(e, "QuantityIn") + ")";
			if(("out").equals(ListToView))
					_rowText = _rowText + "(" + XmlFunctions.getValue(e, "QuantityOut") + ")";
			
			map.put("rowName", "" + _rowText);
			map.put("rowId", "" + XmlFunctions.getValue(e, "Id"));

			fillMaps.add(map);
		}

		// fill in the grid_item layout
		ProductsOfListListMap = fillMaps;
		SimpleAdapter adapter = new SimpleAdapter(this, fillMaps,
				R.layout.product_list_grid_item, from, to);

		lv.setAdapter(adapter);

		// setContentView(R.layout.view_list);
		//lv = (ListView) findViewById(R.id.listView1);
		lv.setOnItemClickListener(new AdapterView.OnItemClickListener() {
			public void onItemClick(AdapterView<?> parent, View view,
					int position, long id) {
				String name = ((TextView) view.findViewById(R.id.textView_ProductList_GridItem_Text))
						.getText().toString();
				String idd = ((TextView) view.findViewById(R.id.textView_ProductList_GridItem_id)).getText()
						.toString();
				Toast.makeText(getApplicationContext(), "Recepy Id " + idd,
						Toast.LENGTH_LONG).show();
//				Intent startNewActivityOpen = new Intent(
//						ViewProductsList.this, ViewRecepyActivity.class);
//				startNewActivityOpen.putExtra("id", idd);
//				startActivityForResult(startNewActivityOpen, 0);
			}
		});
		// mListItem = RecepiesForLV.getItems();
		// listview.setAdapter(new ListAdapter(Splash.this, R.id.list_view,
		// mListItem));

		// if(_isStarting) _isStarting = false;
	}
    
    @Override
	public void onCreateContextMenu(ContextMenu menu, View v,
			ContextMenuInfo menuInfo) {
		// if (v.getId()==R.id.tableLayoutRecepyIngredients) {
		super.onCreateContextMenu(menu, v, menuInfo);
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.activity_view_products_right_menu, menu);
		// }
	}
    
    public boolean onContextItemSelected(MenuItem item) {
		AdapterView.AdapterContextMenuInfo info = (AdapterView.AdapterContextMenuInfo) item
				.getMenuInfo();
		// int menuItemIndex = item.getItemId();
		// String[] menuItems = getResources().getStringArray(R.array.menu);
		// String menuItemName = menuItems[menuItemIndex];
		int _positionListView = info.position;
		
		String _recipeIdToDel = "";
		String _recipeNameToDel = "";

		int index = 0;
		for (HashMap<String, String> entry : ProductsOfListListMap) {
			if (index == _positionListView) {
				Set set = entry.entrySet();

				Iterator i = set.iterator();

				while (i.hasNext()) {
					Map.Entry<String, String> me = (Map.Entry<String, String>) i.next();
					if (me.getKey() == "rowId")
						_recipeIdToDel = me.getValue();
					else
						_recipeNameToDel = me.getValue();
				}

			}
			index++;
		}

		final String _rrecipeIdToDel = _recipeIdToDel;

		if (!_recipeIdToDel.equals("")) {
			new AlertDialog.Builder(ViewProductsList.this)
					.setTitle("Apagar?")
					.setMessage(
							"Tem a certeza que quer apagar a receita : "
									+ _recipeNameToDel)
					.setPositiveButton("Ok",
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {

									GlobalFunctions _gf = new GlobalFunctions();
									_gf.DeleteRecipe(_rrecipeIdToDel);
									//InitializeRecipeCategories();
									//InitializeRecipe("Todas");
								}
							})
					.setNegativeButton("Cancel",
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									// Do nothing.
								}
							}).show();

		}

		return super.onContextItemSelected(item);
	}
	
}
