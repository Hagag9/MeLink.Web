from playwright.sync_api import sync_playwright

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    context = browser.new_context()
    page = context.new_page()

    # Go to the home page
    page.goto("http://localhost:5113")

    # Take a screenshot of the English page
    page.screenshot(path="jules-scratch/verification/english.png")

    # Select Arabic language
    page.select_option('select[name="culture"]', 'ar-SA')

    # Wait for the page to reload
    page.wait_for_load_state('networkidle')

    # Take a screenshot of the Arabic page
    page.screenshot(path="jules-scratch/verification/arabic.png")

    # Close browser
    browser.close()

with sync_playwright() as playwright:
    run(playwright)
