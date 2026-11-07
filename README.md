# MetroBiz Advisor

**Design Document**

## Team Members

- Priya Kachhia
- Alisha Rana
- Raj Aware

## Introduction

Have you ever wondered which neighborhood in your city would be the best place to start a business? Choosing a location can be difficult because there are many factors to consider, such as crime rates, community activity, and neighborhood growth.

MetroBiz Advisor helps users find and compare neighborhoods using public city data. Cincinnati is our launch city, but the design can be reused for any metropolitan area that publishes open data. Instead of searching through different websites and datasets, users can view important information in one place and make better decisions about where to open a business.

MetroBiz Advisor can help users:

- Search neighborhoods by name
- Analyze neighborhoods using crime and development data
- Compare neighborhoods side-by-side
- View business location recommendations
- See neighborhood growth trends
- Save previous analyses
- Connect with other entrepreneurs interested in the same neighborhood
- Access neighborhood data through a JSON API

The goal of this project is to provide users with useful information that can help them evaluate different locations before starting a business.

## Logo

![MetroBiz Advisor Logo]

The logo is a single upward arrow rising out of an open circle. The circle represents a city and its neighborhoods, and the arrow represents growth and business opportunity. Using only two shapes keeps the logo readable at any size, from a browser tab icon to a page header.

## Storyboard

![MetroBiz Advisor Dashboard]

**Design revision:** The Home page will lead with one primary action: choose a business type and a neighborhood, then click Analyze. The header will be reduced in height, and statistics, recommendations, and recent analyses will move below that action so nothing competes with it.

### Home Page

The Home page allows users to:

- Select a business type
- Select a neighborhood
- Run a location analysis
- View neighborhood statistics
- View recommendations
- View recent analyses

### Analyze Page

![Analyze Page]

The Analyze page allows users to select a business type and neighborhood and view crime data, building permit activity, location suitability scores, and trend visualizations.

### Compare Page

![Compare Page]

The Compare page allows users to compare two neighborhoods side-by-side using crime statistics, permit activity, and location suitability scores.

### History Page

![History Page]

The History page allows users to view and search previous business location analyses that have been saved to the database.

### Community Page

The Community page lets users post and reply in a discussion board for each neighborhood and list the type of business they hope to open there. Other entrepreneurs can find them as potential partners, co-founders, or neighbors.

## GitHub Project

GitHub Projects will be used to organize and track development throughout the semester.

Project columns:

- To Do
- In Progress
- Review
- Done

Planned tasks include:

- Set up ASP.NET Core project
- Build Home page
- Build Analyze page
- Build Compare page
- Build History page
- Build Community page (neighborhood discussion boards and entrepreneur profiles)
- Connect crime dataset
- Connect building permit dataset
-
