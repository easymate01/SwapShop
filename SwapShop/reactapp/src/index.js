import React from "react";
import ReactDOM from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";

import "./index.css";
import reportWebVitals from "./reportWebVitals";

import MainPage from "./Pages/MainPage";
import ProductPage from "./Pages/ProductPage/ProductPage";
import Login from "./Pages/Login/Login";
import Registering from "./Pages/Register/Register";
import CreateProduct from "./Pages/CreateProduct/CreateProduct";
import Header from "./Components/Header/Header";

const App = () => {
  const [isLoggedIn, setIsLoggedIn] = React.useState(false);
  const [userName, setUserName] = React.useState("");

  const handleLogin = (username) => {
    setIsLoggedIn(true);
    setUserName(username);
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    setUserName("");
  };

  const router = createBrowserRouter([
    {
      path: "/",
      element: (
        <Header
          isLoggedIn={isLoggedIn}
          userName={userName}
          onLogout={handleLogout}
        />
      ),
      children: [
        {
          path: "/",
          element: <MainPage />,
        },
        {
          path: "/login",
          element: <Login onLogin={handleLogin} />,
        },
        {
          path: "/register",
          element: <Registering />,
        },
        {
          path: "/marketplace",
          element: <MainPage />,
        },
        {
          path: "/product/:id",
          element: <ProductPage />,
        },
        {
          path: "/product/create",
          element: <CreateProduct />,
        },
      ],
    },
  ]);

  return (
    <React.StrictMode>
      <RouterProvider router={router} />
    </React.StrictMode>
  );
};

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(<App />);
reportWebVitals();
