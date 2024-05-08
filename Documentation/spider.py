import requests
from bs4 import BeautifulSoup
import urllib.request
import os

def download_images(url):
    response = requests.get(url)
    soup = BeautifulSoup(response.content, 'html.parser')
    
    img_tags = soup.find_all('img')
    
    if not os.path.exists('images'):
        os.makedirs('images')
    
    for i, img_tag in enumerate(img_tags):
        img_url = img_tag['src']
        img_name = f"{i+1}_pic.jpg"
        
        try:
            urllib.request.urlretrieve(img_url, f"images/{img_name}")
            print(f"Image {i+1} downloaded successfully!")
        except Exception as e:
            print(f"Error downloading image {i+1}: {e}")

# Provide the URL of the site here
url = 'https://gesture-docs.atlassian.net/wiki/spaces/Gesture/pages/98425/Character+And+Abilities+Creating'

download_images(url)
