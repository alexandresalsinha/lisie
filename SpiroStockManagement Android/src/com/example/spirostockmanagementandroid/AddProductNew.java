package com.example.spirostockmanagementandroid;

import java.util.ArrayList;

//import me.tests.GlobalFunctions;
//import me.tests.R;
import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnFocusChangeListener;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;

public class AddProductNew extends Activity {
	
	String BarCode;
	String SpinnerSelectedText;
	
	Button Button_AddProductNew;
	
	String ListToAdd = "out";
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_product_new);
        
//        array_spinner_addToListOptions=new String[3];
//        array_spinner_addToListOptions[0]="Don´t add to Inventory or Cart";
//        array_spinner_addToListOptions[1]="Add to Inventory";
//        array_spinner_addToListOptions[2]="Add to Cart";
        Spinner spinner = (Spinner) findViewById(R.id.spinner_addtolistquestion);
        
        spinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            public void onItemSelected(AdapterView<?> parent, View view, int pos, long id) {
            	SpinnerSelectedText = parent.getItemAtPosition(pos).toString();
            }
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
        //ArrayAdapter adapter = new ArrayAdapter(this, android.R.layout.activity_add_product_new, array_spinner_addToListOptions);
     
        Intent intent = getIntent();
		BarCode = intent.getStringExtra("barCode");
		ListToAdd = intent.getStringExtra("listToAdd");
		
		//set spinner value with the list to add
        if(("in").equals(ListToAdd))
        	spinner.setSelection(1);
        if(("out").equals(ListToAdd))
        	spinner.setSelection(2);
		
		Button_AddProductNew = (Button) findViewById(R.id.button_NewProduct_Add);
        Button_AddProductNew.setOnClickListener(Button_AddProductNew_OnClickListener);
        
        final AutoCompleteTextView _hy = (AutoCompleteTextView)findViewById(R.id.autoCompleteTextView1_NewProduct_Category);
        _hy.setText("");
		_hy.setOnFocusChangeListener(new OnFocusChangeListener() {
			
			public void onFocusChange(View v, boolean hasFocus) {
				// TODO Auto-generated method stub
				_hy.showDropDown();
			};
		});
		
		LoadCategoriesAutoComplete();
    }

    final OnClickListener Button_AddProductNew_OnClickListener = new OnClickListener() {
        public void onClick(final View v) {
        	//TODO input control
        	EditText _EditText_Name = (EditText)findViewById(R.id.editText_NewProduct_Name);
        	AutoCompleteTextView _AutoComplete_Category = (AutoCompleteTextView)findViewById(R.id.autoCompleteTextView1_NewProduct_Category);
        	EditText editText_NewProduct_Brand = (EditText)findViewById(R.id.editText_NewProduct_Brand);
        	EditText _EditText_Price = (EditText)findViewById(R.id.editText_NewProduct_Price);
        	EditText _EditText_PriceWeight = (EditText)findViewById(R.id.editText_NewProduct_PriceWeight);
        	EditText _EditText_Package = (EditText)findViewById(R.id.editText_NewProduct_Package);
        	EditText _EditText_QuantityIn = (EditText)findViewById(R.id.editText_NewProduct_QuantityInList);
        	//Spinner _spinner_addtolistquestion = (Spinner)findViewById(R.id.spinner_addtolistquestion);
        	
        	//_spinner_addtolistquestion.get
        	String _listToAdd = ""; 
        	if(("Add to Inventory").equals(SpinnerSelectedText))
        		_listToAdd = "in";
        	if(("Add to Cart").equals(SpinnerSelectedText))
        		_listToAdd = "out";
        	
        	GlobalFunctions _gb = new GlobalFunctions();
        	_gb.AddProductNew(_EditText_Name.getText().toString(), _AutoComplete_Category.getText().toString(), editText_NewProduct_Brand.getText().toString(), 
        			_EditText_Price.getText().toString(), _EditText_PriceWeight.getText().toString().replace('.', ','), _EditText_Package.getText().toString(), BarCode, _listToAdd, _EditText_QuantityIn.getText().toString());
        	
        	Intent startNewActivityOpen = new Intent(
        			AddProductNew.this, MainActivity.class);
    		startActivityForResult(startNewActivityOpen, 0);
        }
    };
    
    public void LoadCategoriesAutoComplete()
	{
		GlobalFunctions _gf = new GlobalFunctions();
		ArrayList<String> _categories = _gf.GetCategoriesList();
		AutoCompleteTextView textView = (AutoCompleteTextView) findViewById(R.id.autoCompleteTextView1_NewProduct_Category);
		ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, android.R.layout.simple_dropdown_item_1line, _categories);
		textView.setAdapter(adapter);
	}
    
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.activity_add_product_new, menu);
        return true;
    }
}
