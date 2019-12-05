import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import translations from "./translations.js";

// the translations
// (tip move them in a JSON file and import them)
//const resources = {
//	en: {
//		translation: {
//			"Welcome to React": "Welcome to React and react-i18next",
//			"Hello": "Hi there!",
//			"Logout": "Get out of here"
//		}
//	},
//	el: {
//		translation: {
//			"Hello": "Γεια σου!",
//			"Logout": "Αποσύνδεση"
//		}
//	}
//};

const resources = translations;

i18n
	.use(initReactI18next) // passes i18n down to react-i18next
	.init({
		resources,
		lng: "en",

		keySeparator: false, // we do not use keys in form messages.welcome

		interpolation: {
			escapeValue: false // react already safes from xss
		}
	});

export default i18n;