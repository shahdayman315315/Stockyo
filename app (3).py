from flask import Flask, render_template, request, jsonify
import numpy as np
import pickle
import os

app = Flask(__name__)

# Get the directory where this script is located
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
MODEL_PATH = os.path.join(BASE_DIR, 'model.pkl')

# Load your trained model
with open(MODEL_PATH, 'rb') as f:
    model = pickle.load(f)

# Load scaler if it exists (uncomment and use if you have a scaler file)
scaler = None
SCALER_PATH = os.path.join(BASE_DIR, 'scaler.pkl')
if os.path.exists(SCALER_PATH):
    with open(SCALER_PATH, 'rb') as f:
        scaler = pickle.load(f)
    print("Scaler loaded successfully!")
else:
    print("Warning: No scaler found. If you used scaling during training, predictions may be incorrect.")

@app.route('/', methods=['GET'])
def hello_world():
    return render_template('index.html')

@app.route('/submit', methods=['POST'])
def submit_data():
    try:
        # Get form data and convert to appropriate types
        inventory_level = float(request.form.get('inventory_level'))
        units_sold = float(request.form.get('units_sold'))
        units_ordered = float(request.form.get('units_ordered'))
        price = float(request.form.get('price'))
        discount = float(request.form.get('discount'))
        competitor_pricing = float(request.form.get('competitor_pricing'))
        
        # Prepare input for the model
        # Make sure the order matches how your model was trained
        input_features = np.array([[
            inventory_level,
            units_sold,
            units_ordered,
            price,
            discount,
            competitor_pricing
        ]])
        
        # Apply scaling if scaler exists
        if scaler is not None:
            input_features = scaler.transform(input_features)
        
        # Make prediction
        prediction = model.predict(input_features)
        
        # Convert prediction to a Python native type for JSON serialization
        if isinstance(prediction, np.ndarray):
            prediction_value = float(prediction[0])
        else:
            prediction_value = float(prediction)
        
        # Return success response with prediction
        return jsonify({
            'status': 'success',
            'message': 'Prediction completed successfully',
            'prediction': prediction_value,
            'input_data': {
                'inventory_level': inventory_level,
                'units_sold': units_sold,
                'units_ordered': units_ordered,
                'price': price,
                'discount': discount,
                'competitor_pricing': competitor_pricing
            }
        })
    
    except Exception as e:
        # Return error response
        return jsonify({
            'status': 'error',
            'message': f'Error processing data: {str(e)}'
        }), 400

if __name__ == '__main__':
    app.run(port=3000, debug=True)
