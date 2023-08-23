import { useParams, useLocation } from "react-router-dom";
import { Outlet, Link } from "react-router-dom";
import React from "react";

const Header = ({ isLoggedIn, userName, onLogout }) => {
  const location = useLocation();

  return (
    <React.Fragment>
      <header className="app-header">
        <div className="app-header-logo">
          <div className="logo">
            <span className="logo-icon">
              <img src="https://assets.codepen.io/285131/almeria-logo.svg" />
            </span>
            <h1 className="logo-title">
              <span>Swap</span>
              <span>Shop</span>
            </h1>
          </div>
        </div>

        <div className="app-header-navigation">
          <div className="tabs">
            <Link to="/marketplace">Marketplace</Link>

            <Link to="/">Trending Products</Link>

            <Link to="/">Account</Link>

            <Link to="/">Business</Link>
            <Link to="/">System</Link>
          </div>
        </div>

        <div className="app-header-actions">
          {isLoggedIn ? (
            <>
              <li>Hello, {userName}!</li>
              <li>
                <button onClick={onLogout}>Logout</button>
              </li>
            </>
          ) : (
            <>
              <Link to="/login">Login</Link>

              <Link to="/register">Register</Link>
            </>
          )}
          <div className="app-header-actions-buttons">
            <button className="icon-button large">
              <i className="ph-magnifying-glass"></i>
            </button>
            <button className="icon-button large">
              <i className="ph-bell"></i>
            </button>
          </div>
        </div>
        <div className="app-header-mobile">
          <button className="icon-button large">
            <i className="ph-list"></i>
          </button>
        </div>
      </header>
      <Outlet />
    </React.Fragment>
  );
};

export default Header;
