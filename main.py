import csv
import tkinter as tk
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import matplotlib.pyplot as plt
import numpy as np
import json

# Set background color
background_color = '#293241'
plt.rcParams['axes.facecolor'] = background_color
plt.rc('axes', edgecolor='#4f5d75')

d_min = 0.0
d_max = 0.0
num_particles = 0
max_iterations = 0

def read_parameters_from_csv(file_path="C:/Facultate/IA/SwarmOptimizationCLI/parameters.csv"):
        try:
            with open(file_path, newline='') as csvfile:
                reader = csv.DictReader(csvfile)
                for row in reader:
                    # Assuming the CSV file has columns 'NumberOfParticles', 'Dimension', 'MaxIterations'
                    global d_min, d_max, num_particles, max_iterations
                    num_particles = int(row['NumberOfParticles'])
                    dimension = int(row['Dimension'])
                    max_iterations = int(row['MaxIterations'])
                    d_min = float(row['Min'])
                    d_max = float(row["Max"])
                    print("num_particles:%d, dim:%d, max_iter:%d", num_particles, dimension, max_iterations, d_min, d_max);
                    
        except FileNotFoundError:
            print(f"File {file_path} not found.")
        except Exception as e:
            print(f"An error occurred: {e}")

read_parameters_from_csv()


def read_json_data(filename):
    global best_particle, positions

    with open(filename, 'r') as file:
        data = json.load(file)

    best_particle = {
        "Position": data["BestParticle"]["Position"],
        "Velocity": data["BestParticle"]["Velocity"],
        "Cost": data["BestParticle"]["Cost"]
    }

    positions = data["Positions"]
    return best_particle, positions


gbest_particle, gbest_positions = read_json_data("C:/Facultate/IA/SwarmOptimizationCLI/GBEST.json")
lbest_particle, lbest_positions = read_json_data("C:/Facultate/IA/SwarmOptimizationCLI/LBEST.json")

class GraphApp:
    def __init__(self, master, d_min, d_max):
        self.master = master
        self.master.title("Particle Swarm Optimization")

        # Set up Matplotlib figures and axes with specified background color
        self.fig1, self.ax1 = plt.subplots(facecolor=background_color)
        self.fig2, self.ax2 = plt.subplots(facecolor=background_color)

        self.d_min = d_min
        self.d_max = d_max

        # Create canvas for each graph
        self.canvas1 = FigureCanvasTkAgg(self.fig1, master=self.master)
        self.canvas1.draw()
        self.canvas1.get_tk_widget().pack(side=tk.LEFT, fill=tk.BOTH, expand=1)

        self.canvas2 = FigureCanvasTkAgg(self.fig2, master=self.master)
        self.canvas2.draw()
        self.canvas2.get_tk_widget().pack(side=tk.RIGHT, fill=tk.BOTH, expand=1)

        # Initialize data for both graphs
        self.gbest = np.array(gbest_positions[0])
        self.lbest = np.array(lbest_positions[0])

        # Plot the graphs immediately upon app start
        self.plot_graphs()

        # Bind the window close event to a handler
        self.master.protocol("WM_DELETE_WINDOW", self.on_closing)

        # Schedule automatic updates (every 500 milliseconds)
        self.master.after(2000, self.update_graphs)
        

    def plot_graphs(self,):
        
        self.ax1.clear()
        self.ax1.scatter(self.gbest[:, 0], self.gbest[:, 1], color='#bc4b51', s = 10)
        self.ax1.set_title('GBEST')
        self.ax1.set_xlabel('X-axis ')
        self.ax1.set_ylabel('Y-axis ')
        self.ax1.set_xlabel('X-axis', fontsize=8) 
        self.ax1.set_ylabel('Y-axis', fontsize=8)
        self.ax1.set_xlim(self.d_min ,self.d_max )
        self.ax1.set_ylim(self.d_min,self.d_max )

        tick_interval = 0.5
        self.ax1.set_xticks(np.arange(self.d_min, self.d_max + tick_interval, tick_interval))
        self.ax1.set_yticks(np.arange(self.d_min, self.d_max + tick_interval, tick_interval))
        self.ax1.tick_params(axis='both', which='major', labelsize=6, width=0.5)
        # Show grid
        self.ax1.grid(True)

        self.ax2.clear()
        self.ax2.scatter(self.lbest[:, 0], self.lbest[:, 1], color='#8cb369',s = 10)
        self.ax2.set_title('LBEST')
        self.ax2.set_xlabel('X-axis ')
        self.ax2.set_ylabel('Y-axis ')
        self.ax2.set_xlim(self.d_min,self.d_max)
        self.ax2.set_ylim(self.d_min,self.d_max)
        self.ax2.set_xlabel('X-axis', fontsize=8)
        self.ax2.set_ylabel('Y-axis', fontsize=8)

        self.ax2.set_xticks(np.arange(self.d_min, self.d_max + tick_interval, tick_interval))
        self.ax2.set_yticks(np.arange(self.d_min, self.d_max + tick_interval, tick_interval))
        self.ax2.tick_params(axis='both', which='major', labelsize=6, width=0.5)
        
        # Show grid
        self.ax2.grid(True)

        self.canvas1.draw()
        self.canvas2.draw()

    # def update_gbest_graph(self, current_iteration=1):
    #     if current_iteration >= max_iterations:
    #         best_particle_info_gbest = f'Best Particle Cost: {gbest_particle["Cost"]}\nPosition: ({gbest_particle["Position"][0]}, {gbest_particle["Position"][1]})'
    #         self.ax1.text(0.5, -0.1, best_particle_info_gbest, ha='center', va='center', transform=self.ax1.transAxes, bbox=dict(facecolor='white', alpha=0.5))
    #         self.canvas1.draw()
        
    #         return
    #     self.gbest = np.array(gbest_positions[current_iteration])
    #     self.plot_graphs()
    #     # Schedule the next update (every 1 millisecond)
    #     self.master.after(100, self.update_gbest_graph, current_iteration + 1)

    # def update_lbest_graph(self, current_iteration=1):
    #     if current_iteration >= max_iterations:
    #         best_particle_info_lbest = f'Best Particle Cost: {lbest_particle["Cost"]}\nPosition: ({lbest_particle["Position"][0]}, {lbest_particle["Position"][1]})'
    #         self.ax2.text(0.5, -0.1, best_particle_info_lbest, ha='center', va='center', transform=self.ax2.transAxes, bbox=dict(facecolor='white', alpha=0.5))
    #         self.canvas2.draw()
    #         return
    #     # Update data for LBEST graph
    #     self.lbest = np.array(lbest_positions[current_iteration])
    #     self.plot_graphs()
    #     # Schedule the next update (every 15 milliseconds)
    #     self.master.after(100 , self.update_lbest_graph, current_iteration + 1)

    def update_graphs(self, current_iteration = 1):
        if current_iteration >= max_iterations:
            best_particle_info_gbest = f'Best Particle Cost: {gbest_particle["Cost"]}\nPosition: ({gbest_particle["Position"][0]}, {gbest_particle["Position"][1]})'
            self.ax1.text(0.5, -0.1, best_particle_info_gbest, ha='center', va='center', transform=self.ax1.transAxes, bbox=dict(facecolor='white', alpha=0.5))
            self.canvas1.draw()
            best_particle_info_lbest = f'Best Particle Cost: {lbest_particle["Cost"]}\nPosition: ({lbest_particle["Position"][0]}, {lbest_particle["Position"][1]})'
            self.ax2.text(0.5, -0.1, best_particle_info_lbest, ha='center', va='center', transform=self.ax2.transAxes, bbox=dict(facecolor='white', alpha=0.5))
            self.canvas2.draw()
            
            return
        
        self.gbest = np.array(gbest_positions[current_iteration])
        self.lbest = np.array(lbest_positions[current_iteration])
        self.plot_graphs()
        self.master.after(10 ,self.update_graphs, current_iteration + 1)

    def on_closing(self):
        # Disconnect Matplotlib event handlers and close figures
        self.fig1.canvas.mpl_disconnect('all')
        plt.close(self.fig1)
        self.fig2.canvas.mpl_disconnect('all')
        plt.close(self.fig2)
        self.master.destroy()

    
# Create main window and run the app
root = tk.Tk()
app = GraphApp(root, d_min, d_max)
root.mainloop()