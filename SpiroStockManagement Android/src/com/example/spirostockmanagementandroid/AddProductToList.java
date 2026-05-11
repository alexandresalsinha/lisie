package com.example.spirostockmanagementandroid;


//import me.tests.R;

import java.io.File;

import org.w3c.dom.NodeList;

import android.os.Bundle;
import android.R.integer;
import android.app.Activity;
import android.content.Intent;
import android.graphics.BitmapFactory;
import android.view.ContextMenu;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.View;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.TextView;

public class AddProductToList extends Activity {

	int ProductId;
	Button Button_AddProductToList;
	EditText EditText_QuantityToAdd;
	
	String ListToAdd = "out";
	String SpinnerSelectedText;
	
	NodeList ProductNodeList = null;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_product_to_list);
        
        Intent intent = getIntent();
		String _productId = intent.getStringExtra("productId");
		
		ListToAdd = intent.getStringExtra("listToAdd");
		ProductId =  Integer.parseInt(_productId);
		
		Button_AddProductToList = (Button) findViewById(R.id.button_AddProducToList);
		Button_AddProductToList.setOnClickListener(Button_AddProductToList_OnClickListener);
		
		EditText_QuantityToAdd = (EditText) findViewById(R.id.editText_QuantityToAdd);

		
		Spinner spinner = (Spinner) findViewById(R.id.spinner_ProductAddToList_Options);
        
        spinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            public void onItemSelected(AdapterView<?> parent, View view, int pos, long id) {
            	SpinnerSelectedText = parent.getItemAtPosition(pos).toString();
            }
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
        
		//TextView _productBasicInfo = (TextView) findViewById(R.id.textView_BarCodeEntered);
		//_productBasicInfo.setText(_productId);
		
		//Initialize the Product Xml just once
		GlobalFunctions _gb = new GlobalFunctions();
    	ProductNodeList =  _gb.GetNodesOfXpath("//Product[Id='" + ProductId + "']");
    	
		InitializeProductData();
		InitializeSpinnerOptions();
    }

    //On click listener for button1
    final OnClickListener Button_AddProductToList_OnClickListener = new OnClickListener() {
        public void onClick(final View v) {
        	GlobalFunctions _gb = new GlobalFunctions();
        	_gb.AddProductToList(null, ProductId, ListToAdd, EditText_QuantityToAdd.getText().toString());
        	
        	if(("in").equals(ListToAdd))
        	{
        		if(("Add to Inventory and remove from Cart").equals(SpinnerSelectedText))
        		{
        			_gb.RemoveQuantityFromCartOfProduc(ProductId, EditText_QuantityToAdd.getText().toString());
        		}
			}
        	if(("out").equals(ListToAdd))
        	{
        		if(("Add to Cart and remove from Inventory").equals(SpinnerSelectedText))
        		{
        			_gb.RemoveQuantityFromInventoryOfProduct(ProductId, EditText_QuantityToAdd.getText().toString());
        		}
            	
        	}
        	
        	Intent startNewActivityOpen = new Intent(
        			AddProductToList.this, MainActivity.class);
    		startActivityForResult(startNewActivityOpen, 0);
        }
    };
    
    
    void InitializeSpinnerOptions()
    {
    	String array_spinner[];
    	//TODO add the weight part
    	if(("in").equals(ListToAdd))
    	{
    		int _cartCountOfProduct = Integer.parseInt(XmlFunctions.getValue(ProductNodeList.item(0),"QuantityOut"));
    		if (_cartCountOfProduct > 0) 
    		{
    			array_spinner=new String[2];
                array_spinner[0]="Add to Inventory";
                array_spinner[1]="Add to Inventory and remove from Cart";
                
                Spinner s = (Spinner) findViewById(R.id.spinner_ProductAddToList_Options);
                ArrayAdapter adapter = new ArrayAdapter(this,
                android.R.layout.simple_spinner_item, array_spinner);
                s.setAdapter(adapter);
			}
    		else 
    		{
    			array_spinner=new String[1];
                array_spinner[0]="Add to Inventory";
                
                Spinner s = (Spinner) findViewById(R.id.spinner_ProductAddToList_Options);
                ArrayAdapter adapter = new ArrayAdapter(this,
                android.R.layout.simple_spinner_item, array_spinner);
                s.setAdapter(adapter);
    		}			
    	}
    	if(("out").equals(ListToAdd))
    	{
    		int _inventoryCountOfProduct = Integer.parseInt(XmlFunctions.getValue(ProductNodeList.item(0),"QuantityIn"));
    		if (_inventoryCountOfProduct > 0) 
    		{
    			array_spinner=new String[2];
                array_spinner[0]="Add to Cart";
                array_spinner[1]="Add to Cart and remove from Inventory";
                
                Spinner s = (Spinner) findViewById(R.id.spinner_ProductAddToList_Options);
                ArrayAdapter adapter = new ArrayAdapter(this,
                android.R.layout.simple_spinner_item, array_spinner);
                s.setAdapter(adapter);
			}
    		else 
    		{
    			array_spinner=new String[1];
                array_spinner[0]="Add to Cart";
                
                Spinner s = (Spinner) findViewById(R.id.spinner_ProductAddToList_Options);
                ArrayAdapter adapter = new ArrayAdapter(this,
                android.R.layout.simple_spinner_item, array_spinner);
                s.setAdapter(adapter);
    		}
    	}
    	
        
    }
    
    void InitializeProductData()
    {
    	
    	if (ProductNodeList.getLength() > 0) 
    	{
    		//Fill the data
    		//String _pproductId = XmlFunctions.getValue(ProductNodeList.item(0),"Id");
    		
    		String path = "/storage/extSdCard/SpiroSMDatabases/ItemsImages/" + ProductId + "small.jpg";              
    		File file = new File(path);
    	    if(file.exists()) 
    	    {
    	    	ImageView _image = (ImageView)findViewById(R.id.imageView_ProductImage);
    	    	_image.setImageBitmap(BitmapFactory.decodeFile(path));
    	    }
    		
    		TextView _tvName = (TextView)findViewById(R.id.textView_ProductName);
    		_tvName.setText(XmlFunctions.getValue(ProductNodeList.item(0),"Name"));
    		
    		TextView _tvPrice = (TextView)findViewById(R.id.textView_ProductPrice);
    		_tvPrice.setText(XmlFunctions.getValue(ProductNodeList.item(0),"Price"));
    		
    		TextView _tvProductPriceWeight = (TextView)findViewById(R.id.textView_ProductPriceWeight);
    		_tvProductPriceWeight.setText(XmlFunctions.getValue(ProductNodeList.item(0),"VariableWeightPrice"));
    		
    		TextView _tvProductCategory = (TextView)findViewById(R.id.textView_ProductCategory);
    		_tvProductCategory.setText(XmlFunctions.getValue(ProductNodeList.item(0),"categoryString"));
    		
    		TextView _tvPackageQuantity = (TextView)findViewById(R.id.textView_PackageQuantity);
    		_tvPackageQuantity.setText(XmlFunctions.getValue(ProductNodeList.item(0),"PackageInfo"));
    		
    		
    		TextView _tvCurrentInShoppingCart = (TextView)findViewById(R.id.textView_CurrentInShoppingCart);
    		_tvCurrentInShoppingCart.setText("Currently on Shopping Cart: " + XmlFunctions.getValue(ProductNodeList.item(0),"QuantityOut"));
    		
    		TextView _tvCurrentInInventory = (TextView)findViewById(R.id.textView_CurrentInInventory);
    		_tvCurrentInInventory.setText("Currently on Inventory: " + XmlFunctions.getValue(ProductNodeList.item(0),"QuantityIn"));	
    	}
    }
    
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.activity_add_product_to_list, menu);
        return true;
    }
}
